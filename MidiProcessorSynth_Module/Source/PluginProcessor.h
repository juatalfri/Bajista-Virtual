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
    void pauseMidi(int type);

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

    //Segundos que se movera el archivo midi
    double rewindForwardSeconds = 0;

    //.txt que contiene la ruta del archivo midi
    File MidiTxtPath = File::getCurrentWorkingDirectory().getChildFile("./Assets/StreamingAssets/MidiPath.txt"); //Unity Editor
    //File MidiTxtPath = File::getCurrentWorkingDirectory().getChildFile("./Bajista Virtual_Data/StreamingAssets/MidiPath.txt"); //Unity Build
    //File MidiTxtPath = File::getCurrentWorkingDirectory().getChildFile("../../Media/MidiPath.txt"); //debug

    //Flag para controlar cambio de archivo midi
    AudioParameterFloat* midiFileChanged;

    //Flag para controlar pausa de archivo midi
    AudioParameterFloat* midiFilePaused;

    //Flag para controlar avance o retroceso de archivo midi
    AudioParameterFloat* rewindForward;

    //Track seleccionado
    AudioParameterFloat* currentTrack;

    //==============================================================================
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MidiProcessorSynth_ModuleAudioProcessor)
};
