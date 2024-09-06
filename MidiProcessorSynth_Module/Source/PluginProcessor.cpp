/*
  ==============================================================================

    This file contains the basic framework code for a JUCE plugin processor.

  ==============================================================================
*/

#include "PluginProcessor.h"
#include <Windows.h>

// Vectores con las notas midi y sus timestamps
std::vector<int> notesNumber;
std::vector<double> timestamps;

//==============================================================================
MidiProcessorSynth_ModuleAudioProcessor::MidiProcessorSynth_ModuleAudioProcessor()
    : AudioProcessor(BusesProperties()
        .withOutput("Output", juce::AudioChannelSet::stereo(), true)
    )
{
    addParameter(midiFileStarted = new AudioParameterFloat("Iniciar Midi", "Iniciar Midi", 0, 1, 0));
    addParameter(midiFilePaused = new AudioParameterFloat("Pausar Midi", "Pausar Midi", 0, 1, 0));
    addParameter(midiFileEnded = new AudioParameterFloat("Finalizar Midi", "Finalizar Midi", 0, 1, 0));
    addParameter(currentTrack = new AudioParameterFloat("Pista Bajo", "Pista Bajo", 0, 20, 0));
    addParameter(selectAllTracks = new AudioParameterFloat("Cancion completa", "Cancion completa", 0, 1, 0));
    addParameter(noteVelocity = new AudioParameterFloat("Velocidad", "Velocidad", 0, 1, 1));
}

MidiProcessorSynth_ModuleAudioProcessor::~MidiProcessorSynth_ModuleAudioProcessor()
{
}

//==============================================================================
const juce::String MidiProcessorSynth_ModuleAudioProcessor::getName() const
{
    return JucePlugin_Name;
}

bool MidiProcessorSynth_ModuleAudioProcessor::acceptsMidi() const
{
#if JucePlugin_WantsMidiInput
    return true;
#else
    return false;
#endif
}

bool MidiProcessorSynth_ModuleAudioProcessor::producesMidi() const
{
#if JucePlugin_ProducesMidiOutput
    return true;
#else
    return false;
#endif
}

bool MidiProcessorSynth_ModuleAudioProcessor::isMidiEffect() const
{
#if JucePlugin_IsMidiEffect
    return true;
#else
    return false;
#endif
}

double MidiProcessorSynth_ModuleAudioProcessor::getTailLengthSeconds() const
{
    return 0.0;
}

int MidiProcessorSynth_ModuleAudioProcessor::getNumPrograms()
{
    return 1;   // NB: some hosts don't cope very well if you tell them there are 0 programs,
    // so this should be at least 1, even if you're not really implementing programs.
}

int MidiProcessorSynth_ModuleAudioProcessor::getCurrentProgram()
{
    return 0;
}

void MidiProcessorSynth_ModuleAudioProcessor::setCurrentProgram(int index)
{
}

const juce::String MidiProcessorSynth_ModuleAudioProcessor::getProgramName(int index)
{
    return {};
}

void MidiProcessorSynth_ModuleAudioProcessor::changeProgramName(int index, const juce::String& newName)
{
}

//==============================================================================
void MidiProcessorSynth_ModuleAudioProcessor::prepareToPlay(double sampleRate, int samplesPerBlock)
{
    setUsingSampledSound();

    midiCollector.reset(sampleRate);
    synth.setCurrentPlaybackSampleRate(sampleRate);
}

void MidiProcessorSynth_ModuleAudioProcessor::releaseResources()
{
    // When playback stops, you can use this as an opportunity to free up any
    // spare memory, etc.
}

#ifndef JucePlugin_PreferredChannelConfigurations
bool MidiProcessorSynth_ModuleAudioProcessor::isBusesLayoutSupported(const BusesLayout& layouts) const
{
#if JucePlugin_IsMidiEffect
    juce::ignoreUnused(layouts);
    return true;
#else
    // This is the place where you check if the layout is supported.
    // In this template code we only support mono or stereo.
    // Some plugin hosts, such as certain GarageBand versions, will only
    // load plugins that support stereo bus layouts.
    if (layouts.getMainOutputChannelSet() != juce::AudioChannelSet::mono()
        && layouts.getMainOutputChannelSet() != juce::AudioChannelSet::stereo())
        return false;

    // This checks if the input layout matches the output layout
#if ! JucePlugin_IsSynth
    if (layouts.getMainOutputChannelSet() != layouts.getMainInputChannelSet())
        return false;
#endif

    return true;
#endif
}
#endif

void MidiProcessorSynth_ModuleAudioProcessor::processBlock(juce::AudioBuffer<float>& buffer, juce::MidiBuffer& midiMessages)
{
    //Reinicia los vectores con la informacion del midi y prepara el buffer con los nuevos datos
    if (midiFileStarted->get() == 1)
    {
        synth.allNotesOff(0, false);
        midiBuffer.clear();
        timestamps.clear();
        notesNumber.clear();

        File selectedMidi = MidiTxtPath.loadFileAsString();

        if (selectedMidi.existsAsFile())
        {
            // Lee archivo midi
            juce::FileInputStream fileStream(selectedMidi);
            midiFile.readFrom(fileStream);
            midiFile.convertTimestampTicksToSeconds();

            loadMidiData();
            waitForAnimation();
        }
        resumeSeconds = Time::getMillisecondCounterHiRes() / 1000;
    }
    //Pausa la reproducción
    else if (midiFilePaused->get() == 1 && !paused)
    {
        paused = true;
        pauseSeconds += Time::getMillisecondCounterHiRes() / 1000 - resumeSeconds;

        midiBuffer.clear();
        synth.allNotesOff(0, false);

    }
    else if (midiFilePaused->get() == 0 && paused)
    {
        paused = false;
        loadMidiData();
        resumeSeconds = Time::getMillisecondCounterHiRes() / 1000;
    }
    //Reinicio todos los parametros
    else if (midiFileEnded->get() == 1)
    {
        synth.allNotesOff(0, false);
        midiBuffer.clear();
        timestamps.clear();
        notesNumber.clear();
        
        pauseSeconds = 0;
        resumeSeconds = 0;
        paused = false;
    }

    int numSamples = buffer.getNumSamples();
    int sampleDeltaToAdd = -samplesPlayed;
    midiMessages.addEvents(midiBuffer, samplesPlayed, numSamples, sampleDeltaToAdd);
    samplesPlayed += numSamples;
    currentPositionSeconds = samplesPlayed / getSampleRate();

    midiCollector.removeNextBlockOfMessages(midiMessages, numSamples);

    synth.renderNextBlock(buffer, midiMessages, 0, numSamples);

}

//==============================================================================
bool MidiProcessorSynth_ModuleAudioProcessor::hasEditor() const
{
    return true; // (change this to false if you choose to not supply an editor)
}

juce::AudioProcessorEditor* MidiProcessorSynth_ModuleAudioProcessor::createEditor()
{
    return new GenericAudioProcessorEditor(*this);
}

//==============================================================================
void MidiProcessorSynth_ModuleAudioProcessor::getStateInformation(juce::MemoryBlock& destData)
{
    // You should use this method to store your parameters in the memory block.
    // You could do that either as raw data, or use the XML or ValueTree classes
    // as intermediaries to make it easy to save and load complex data.
}

void MidiProcessorSynth_ModuleAudioProcessor::setStateInformation(const void* data, int sizeInBytes)
{
    // You should use this method to restore your parameters from this memory block,
    // whose contents will have been created by the getStateInformation() call.
}

//==============================================================================
//==============================================================================
//==============================================================================

// Inicializa el audio .wav que sonar para cada nota que pase por el synth
void MidiProcessorSynth_ModuleAudioProcessor::setUsingSampledSound()
{
    // Voces que hacen sonar al synth
    for (auto i = 0; i < 8; ++i)
    {
        synth.addVoice(new SamplerVoice());    // Voz de tipo Sample
    }
    WavAudioFormat wavFormat;
    File filePath = File::getCurrentWorkingDirectory().getChildFile("./Assets/StreamingAssets/one-note-nylon-synth-guitar_C.wav"); //Unity Editor
    //File filePath = File::getCurrentWorkingDirectory().getChildFile("./Bajista Virtual_Data/StreamingAssets/one-note-nylon-synth-guitar_C.wav"); //Unity Build
    //File filePath = File::getCurrentWorkingDirectory().getChildFile("../../Media/one-note-nylon-synth-guitar_C.wav"); //debug

    if (filePath.existsAsFile()) {
        std::unique_ptr<InputStream> sample = filePath.createInputStream();
        std::unique_ptr<AudioFormatReader> audioReader(wavFormat.createReaderFor(sample.release(), true));

        BigInteger allNotes;
        allNotes.setRange(0, 128, true);

        synth.clearSounds();
        synth.addSound(new SamplerSound("Sample sound",
            *audioReader,
            allNotes,
            72,   // nota midi base
            0.1,  // ataque de la nota en segundos
            0.1,  // fade-out en segundos
            10.0  // longitud maxima de la muestra en segundos
        ));
    }
    else {
        Beep(500, 500);
    }
}

// Obtiene el track seleccionado, mete los mensajes midi (notas) en un vector
// y obtiene datos de interes (numero de las notas midi y sus timestamps)
void MidiProcessorSynth_ModuleAudioProcessor::loadMidiData() {
    if (selectAllTracks->get() == 1)
    {
        for (int index = 0; index < midiFile.getNumTracks(); index++)
        {
            const juce::MidiMessageSequence* track = midiFile.getTrack(index);

            for (int i = 0; i < track->getNumEvents(); i++)
            {
                juce::MidiMessage& msg = track->getEventPointer(i)->message;
                if (msg.isNoteOnOrOff()) {
                    if (currentTrack->get() == index)
                    {
                        timestamps.push_back(msg.getTimeStamp() / noteVelocity->get());
                        notesNumber.push_back(msg.getNoteNumber() / noteVelocity->get());
                    }

                    double samplePosition = getSampleRate() * (msg.getTimeStamp() / noteVelocity->get() + currentPositionSeconds - pauseSeconds);
                    midiBuffer.addEvent(msg, samplePosition);
                }
            }
        }
    }
    else
    {
        const juce::MidiMessageSequence* track = midiFile.getTrack(currentTrack->get());
        //const juce::MidiMessageSequence* track = midiFile.getTrack(7); //debug

        for (int i = 0; i < track->getNumEvents(); i++)
        {
            juce::MidiMessage& msg = track->getEventPointer(i)->message;
            if (msg.isNoteOnOrOff()) {
                timestamps.push_back(msg.getTimeStamp() / noteVelocity->get());
                notesNumber.push_back(msg.getNoteNumber() / noteVelocity->get());

                double samplePosition = getSampleRate() * (msg.getTimeStamp() / noteVelocity->get() + currentPositionSeconds - pauseSeconds);
                midiBuffer.addEvent(msg, samplePosition);
            }
        }
    }
}

//Mantiene al processBlock ocupado para pausar el audio hasta que la animación esté preparada
void MidiProcessorSynth_ModuleAudioProcessor::waitForAnimation()
{
    while (midiFileStarted->get() == 1) { Thread::sleep(0); };
}

//Devuelve las notas y sus timestamps mediante PInvoke
extern "C" _declspec(dllexport) void getNotesAndTimestaps(int** noteNumberArray, int* noteNumberSize, double** timestampsArray, int* timestampsSize) {

    *noteNumberSize = notesNumber.size();
    *noteNumberArray = new int[*noteNumberSize];
    std::copy(notesNumber.begin(), notesNumber.end(), *noteNumberArray);

    *timestampsSize = timestamps.size();
    *timestampsArray = new double[*timestampsSize];
    std::copy(timestamps.begin(), timestamps.end(), *timestampsArray);
}

//==============================================================================
// This creates new instances of the plugin..
juce::AudioProcessor* JUCE_CALLTYPE createPluginFilter()
{
    return new MidiProcessorSynth_ModuleAudioProcessor();
}
