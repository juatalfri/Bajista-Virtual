/*
  ==============================================================================

    This file contains the basic framework code for a JUCE plugin processor.

  ==============================================================================
*/

#include "PluginProcessor.h"
#include <Windows.h>

//Numero de tracks totales en el archivo midi
int numTracks = 0;

// Vectores con las notas midi y sus timestamps
std::vector<int> notesNumber;
std::vector<double> timestamps;

//==============================================================================
MidiProcessorSynth_ModuleAudioProcessor::MidiProcessorSynth_ModuleAudioProcessor()
    : AudioProcessor(BusesProperties()
        .withOutput("Output", juce::AudioChannelSet::stereo(), true)
    )
{
    addParameter(midiFileChanged = new AudioParameterFloat("Midi cambiado", "Midi cambiado", 0, 1, 0));
    addParameter(midiFilePaused = new AudioParameterFloat("Midi pausado", "Midi pausado", 0, 1, 0));
    addParameter(rewindForward = new AudioParameterFloat("RetrocederAvanzar", "RetrocederAvanzar", -50, 50, 0));
    addParameter(currentTrack = new AudioParameterFloat("Pista seleccionada", "Pista seleccionada", 0, 20, 0));
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
    if (midiFileChanged->get() == 1)
    {
        synth.allNotesOff(0, false);

        //Reinicia los vectores con la informacion del midi y prepara el buffer con los nuevos datos
        #pragma region Cargar Nuevo Midi
        
        midiBuffer.clear();
        timestamps.clear();
        notesNumber.clear();

        File selectedMidi = MidiTxtPath.loadFileAsString();

        if (selectedMidi.existsAsFile())
        {
            // Lee archivo midi y obtiene el numero de tracks
            #pragma region Leer Midi

            juce::FileInputStream fileStream(selectedMidi);
            midiFile.readFrom(fileStream);
            midiFile.convertTimestampTicksToSeconds();

            numTracks = midiFile.getNumTracks();
            #pragma endregion

            pauseMidi(0);

            // Obtiene el track seleccionado, mete los mensajes midi (notas) en un vector
            // y obtiene datos de interes (numero de las notas midi y sus timestamps)
            #pragma region Procesar Midi

            const juce::MidiMessageSequence* track = midiFile.getTrack(currentTrack->get());
            //const juce::MidiMessageSequence* track = midiFile.getTrack(7); //debug

            for (int i = 0; i < track->getNumEvents(); i++)
            {
                juce::MidiMessage& msg = track->getEventPointer(i)->message;
                if (msg.isNoteOnOrOff()) {
                    timestamps.push_back(msg.getTimeStamp() + currentPositionSeconds);
                    notesNumber.push_back(msg.getNoteNumber());

                    double samplePosition = getSampleRate() * (msg.getTimeStamp() + currentPositionSeconds);
                    midiBuffer.addEvent(msg, samplePosition);
                }
            }
            #pragma endregion
        }
            else {
        Beep(500, 500);
    }
        #pragma endregion
    }
    else if (midiFilePaused->get() == 1)
    {
        synth.allNotesOff(0, false);
        pauseMidi(1);
    }
    else if (rewindForward->get() != 0)
    {
        rewindForwardSeconds = rewindForward->get();
        synth.allNotesOff(0, false);

        //Reinicia los vectores con la informacion del midi y prepara el buffer con los nuevos datos
        #pragma region Cargar Midi con modificacion de tiempo

        midiBuffer.clear();
        timestamps.clear();
        notesNumber.clear();

        pauseMidi(2);

        const juce::MidiMessageSequence* track = midiFile.getTrack(currentTrack->get());
        //const juce::MidiMessageSequence* track = midiFile.getTrack(7); //debug

        for (int i = 0; i < track->getNumEvents(); i++)
        {
            juce::MidiMessage& msg = track->getEventPointer(i)->message;
            if (msg.isNoteOnOrOff()) {
                timestamps.push_back(msg.getTimeStamp() + currentPositionSeconds + rewindForwardSeconds);
                notesNumber.push_back(msg.getNoteNumber());

                double samplePosition = getSampleRate() * (msg.getTimeStamp() + currentPositionSeconds + rewindForwardSeconds);
                midiBuffer.addEvent(msg, samplePosition);
            }
        }
        #pragma endregion
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
    for (auto i = 0; i < 4; ++i)
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

//Mantiene al processBlock ocupado para pausar el audio
void MidiProcessorSynth_ModuleAudioProcessor::pauseMidi(int type)
{
    switch (type)
    {
    case 0: while (currentTrack->get() == 0) { Thread::sleep(0); } break;
    case 1: while (midiFilePaused->get() == 1) { Thread::sleep(0); } break;
    case 2: while (rewindForward->get() != 0) { Thread::sleep(0); } break;
    default: break;
    }
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

//Devuelve num de canales del midi mediante PInvoke
extern "C" _declspec(dllexport) int getNumTracks() {
    
    return numTracks;
}

//==============================================================================
// This creates new instances of the plugin..
juce::AudioProcessor* JUCE_CALLTYPE createPluginFilter()
{
    return new MidiProcessorSynth_ModuleAudioProcessor();
}
