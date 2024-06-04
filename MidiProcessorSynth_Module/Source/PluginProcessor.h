/*
  ==============================================================================

    This file contains the basic framework code for a JUCE plugin processor.

  ==============================================================================
*/

#pragma once

#include <JuceHeader.h>

//==============================================================================
/**
*/
class MidiProcessorSynth_ModuleAudioProcessor  : public juce::AudioProcessor
{
public:
    //==============================================================================
    MidiProcessorSynth_ModuleAudioProcessor();
    ~MidiProcessorSynth_ModuleAudioProcessor() override;

    //==============================================================================
    void prepareToPlay (double sampleRate, int samplesPerBlock) override;
    void releaseResources() override;

   #ifndef JucePlugin_PreferredChannelConfigurations
    bool isBusesLayoutSupported (const BusesLayout& layouts) const override;
   #endif

    void processBlock (juce::AudioBuffer<float>&, juce::MidiBuffer&) override;

    //==============================================================================
    juce::AudioProcessorEditor* createEditor() override;
    bool hasEditor() const override;

    //==============================================================================
    const juce::String getName() const override;

    bool acceptsMidi() const override;
    bool producesMidi() const override;
    bool isMidiEffect() const override;
    double getTailLengthSeconds() const override;

    //==============================================================================
    int getNumPrograms() override;
    int getCurrentProgram() override;
    void setCurrentProgram (int index) override;
    const juce::String getProgramName (int index) override;
    void changeProgramName (int index, const juce::String& newName) override;

    //==============================================================================
    void getStateInformation (juce::MemoryBlock& destData) override;
    void setStateInformation (const void* data, int sizeInBytes) override;

    //==============================================================================
    void setUsingSampledSound();
    void readMidi(const juce::File fileName);
    void processMidi();

    //==============================================================================
private:
    //==============================================================================
    // Recoge mensajes midi en tiempo real desde el midi input device y
    // los convierte en bloques a procesar en el audio callback.
    MidiMessageCollector midiCollector;

    // El sintetizador
    Synthesiser synth;

    // El archivo midi a procesar
    juce::MidiFile midiFile;

    // Vector con todos los mensajes midi del archivo
    std::vector<juce::MidiMessage> notes;

    // Buffer con todos los mensajes midi del archivo
    MidiBuffer midiBuffer;

    //Notas tocadas
    int samplesPlayed = 0;

    //Track seleccionado
    std::atomic<int> currentTrack;

    //Numero de tracks totales en el archivo midi
    std::atomic<int> numTracks;

    // Vector con las notas midi asociadas a su timestamp
    std::vector<std::pair<int, double>> noteTimestamp;

    //==============================================================================
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MidiProcessorSynth_ModuleAudioProcessor)
};
