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
    //void readMidi(File fileName);
    //void processMidi(int sampleRate);
    //void loadMidi(int sampleRate);
    void pauseMidi(bool pause);

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

    // Buffer con todos los mensajes midi del archivo
    MidiBuffer midiBuffer;

    //Notas tocadas
    int samplesPlayed = 0;

    //Posición en segundos del processBlock
    double currentPositionSeconds = 0;

    //.txt que contiene la ruta del archivo midi
    File MidiTxtPath = File::getCurrentWorkingDirectory().getChildFile("./Assets/Media/MidiPath.txt");
    //File MidiTxtPath = File::getCurrentWorkingDirectory().getChildFile("../../Media/MidiPath.txt"); //debug

    //Flag para controlar cambio de archivo midi
    AudioParameterFloat* midiFileChanged;

    //Flag para controlar pausa de archivo midi
    AudioParameterFloat* midiFilePaused;

    //Track seleccionado
    AudioParameterFloat* currentTrack;

    //==============================================================================
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MidiProcessorSynth_ModuleAudioProcessor)
};
