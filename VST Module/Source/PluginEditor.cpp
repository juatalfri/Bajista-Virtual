/*
  ==============================================================================

    This file contains the basic framework code for a JUCE plugin editor.

  ==============================================================================
*/

#include "PluginProcessor.h"
#include "PluginEditor.h"

//==============================================================================
VSTModuleAudioProcessorEditor::VSTModuleAudioProcessorEditor (VSTModuleAudioProcessor& p)
    : AudioProcessorEditor (&p), audioProcessor (p)
{
    // Make sure that before the constructor has finished, you've set the
    // editor's size to whatever you need it to be.
    setSize (500, 500);
}

VSTModuleAudioProcessorEditor::~VSTModuleAudioProcessorEditor()
{
}

//==============================================================================
void VSTModuleAudioProcessorEditor::paint (juce::Graphics& g)
{
    // (Our component is opaque, so we must completely fill the background with a solid colour)
    g.fillAll(getLookAndFeel().findColour(juce::ResizableWindow::backgroundColourId));

    juce::MidiFile song;
    juce::File filePath = juce::File::getCurrentWorkingDirectory().getChildFile("../../Media/Michael_Jackson_-_Billie_Jean (Track 7 Bajo).mid");

    juce::FileInputStream songStream(filePath);
    song.readFrom(songStream);
    song.convertTimestampTicksToSeconds();
    juce::MidiMessageSequence bassTrack = *song.getTrack(7);

    g.setColour(juce::Colours::white);
    g.setFont(15.0f);
    g.drawFittedText("Este archivo MIDI contiene " + std::to_string(song.getNumTracks()) + " canales", getLocalBounds(), juce::Justification::centred, 1);
}

void VSTModuleAudioProcessorEditor::resized()
{
    // This is generally where you'll want to lay out the positions of any
    // subcomponents in your editor..
}
