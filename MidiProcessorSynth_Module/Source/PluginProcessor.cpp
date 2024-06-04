/*
  ==============================================================================

    This file contains the basic framework code for a JUCE plugin processor.

  ==============================================================================
*/

#include "PluginProcessor.h"

//==============================================================================
MidiProcessorSynth_ModuleAudioProcessor::MidiProcessorSynth_ModuleAudioProcessor()
    : AudioProcessor(BusesProperties()
        .withOutput("Output", juce::AudioChannelSet::stereo(), true)
    )
{
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

    File seleccion = "C://Users//juano//Escritorio//TFG//Mi TFG//Bajista-Virtual//MidiProcessorSynth_Module//Media//Midi samples//Billie_Jean_(T7).mid";

    if (seleccion.existsAsFile())
    {
        readMidi(seleccion);
        currentTrack.store(7);
        processMidi();
    }

    midiCollector.reset(sampleRate);
    synth.setCurrentPlaybackSampleRate(sampleRate);

    midiBuffer.clear();
    for (int i = 0; i < notes.size(); i++)
    {
        MidiMessage note = notes[i];
        double samplePosition = sampleRate * note.getTimeStamp();
        midiBuffer.addEvent(note, samplePosition);
    }
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
    juce::ScopedNoDenormals noDenormals;
    int numSamples = buffer.getNumSamples();

    int sampleDeltaToAdd = -samplesPlayed;
    midiMessages.addEvents(midiBuffer, samplesPlayed, numSamples, sampleDeltaToAdd);
    samplesPlayed += numSamples;

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
    //auto filePath = File::getCurrentWorkingDirectory().getChildFile("../../Media/one-note-nylon-synth-guitar_C.wav");
    File filePath = "C://Users//juano//Escritorio//TFG//Mi TFG//Bajista-Virtual//MidiProcessorSynth_Module//Media//one-note-nylon-synth-guitar_C.wav";
    jassert(filePath.existsAsFile());
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

// Lee archivo midi y obtiene el numero de tracks
void MidiProcessorSynth_ModuleAudioProcessor::readMidi(const juce::File fileName)
{
    juce::FileInputStream fileStream(fileName);
    midiFile.readFrom(fileStream);
    midiFile.convertTimestampTicksToSeconds();

    numTracks = midiFile.getNumTracks();
}

// Obtiene el track seleccionado, mete los mensajes midi (notas) en un vector
// y obtiene datos de interes (numero de las notas midi)
void MidiProcessorSynth_ModuleAudioProcessor::processMidi()
{
    const juce::MidiMessageSequence* track = midiFile.getTrack(currentTrack);
    for (int i = 0; i < track->getNumEvents(); i++)
    {
        juce::MidiMessage& msg = track->getEventPointer(i)->message;
        if (msg.isNoteOnOrOff()) {
            noteTimestamp.push_back(std::pair<int, double>(msg.getNoteNumber(), msg.getTimeStamp()));
            notes.push_back(msg);
        }
    }
}
//==============================================================================
// This creates new instances of the plugin..
juce::AudioProcessor* JUCE_CALLTYPE createPluginFilter()
{
    return new MidiProcessorSynth_ModuleAudioProcessor();
}
