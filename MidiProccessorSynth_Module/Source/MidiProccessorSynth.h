#pragma once

#include "AudioLiveScrollingDisplay.h"


//==============================================================================
struct SynthAudioSource final : public AudioSource
{
    SynthAudioSource()
    {
        // Voces que hacen sonar al synth
        for (auto i = 0; i < 4; ++i)
        {
            synth.addVoice(new SamplerVoice());    // Voz de tipo Sample
        }

        setUsingSampledSound();
    }

    // Inizializa el audio .wav que sonar para cada nota que pase por el synth
    void setUsingSampledSound()
    {
        WavAudioFormat wavFormat;
        auto filePath = juce::File::getCurrentWorkingDirectory().getChildFile("../../Media/one-note-nylon-synth-guitar_C.wav");
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
    void readMidi(const juce::File fileName)
    {
        juce::FileInputStream fileStream(fileName);
        midiFile.readFrom(fileStream);
        midiFile.convertTimestampTicksToSeconds();

        numTracks = midiFile.getNumTracks();
    }

    // Obtiene el track seleccionado, mete los mensajes midi (notas) en un vector
    // y obtiene datos de interes (numero de las notas midi)
    void processMidi() 
    {
        const juce::MidiMessageSequence* track = midiFile.getTrack(currentTrack);
        for (int i = 0; i < track->getNumEvents(); i++)
        {
            juce::MidiMessage& msg = track->getEventPointer(i)->message;
            if (msg.isNoteOnOrOff()) {
                noteTimestamp.push_back(std::pair<int, double>(msg.getNoteNumber(), msg.getTimeStamp()));
                midiMessages.push_back(msg);
            }
        }
    }

    // Prepara el synth para comenzar a sonar, normalizando las notas y añadiendolas a un midibuffer
    void prepareToPlay(int samplesPerBlockExpected, double sampleRate)
    {
        midiCollector.reset(sampleRate);
        synth.setCurrentPlaybackSampleRate(sampleRate);

        midiBuffer.clear();
        for (int i = 0; i < midiMessages.size(); i++)
        {
            auto note = midiMessages[i];
            auto samplePosition = sampleRate * note.getTimeStamp();
            midiBuffer.addEvent(note, samplePosition);
        }
    }

    void releaseResources() override {}

    // Procesa el midibuffer y pasa las notas al synth en bloques
    void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill) override
    {
        bufferToFill.clearActiveBufferRegion();

        juce::MidiBuffer incomingMidi;

        int sampleDeltaToAdd = -samplesPlayed;
        incomingMidi.addEvents(midiBuffer, samplesPlayed, bufferToFill.numSamples, sampleDeltaToAdd);
        samplesPlayed += bufferToFill.numSamples;

        midiCollector.removeNextBlockOfMessages(incomingMidi, bufferToFill.numSamples);

        synth.renderNextBlock(*bufferToFill.buffer, incomingMidi, 0, bufferToFill.numSamples);
    }

    //==============================================================================

    // Recoge mensajes midi en tiempo real desde el midi input device y
    // los convierte en bloques a procesar en el audio callback.
    MidiMessageCollector midiCollector;

    // El sintetizador
    Synthesiser synth;

    // El archivo midi a procesar
    juce::MidiFile midiFile;

    // Vector con todos los mensajes midi del archivo
    std::vector<juce::MidiMessage> midiMessages;

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
};

//==============================================================================

class Callback final : public AudioIODeviceCallback
{
public:
    Callback(AudioSourcePlayer& playerIn, LiveScrollingAudioDisplay& displayIn)
        : player(playerIn), display(displayIn) {}

    void audioDeviceIOCallbackWithContext(const float* const* inputChannelData,
        int numInputChannels,
        float* const* outputChannelData,
        int numOutputChannels,
        int numSamples,
        const AudioIODeviceCallbackContext& context) override
    {
        player.audioDeviceIOCallbackWithContext(inputChannelData,
            numInputChannels,
            outputChannelData,
            numOutputChannels,
            numSamples,
            context);
        display.audioDeviceIOCallbackWithContext(outputChannelData,
            numOutputChannels,
            nullptr,
            0,
            numSamples,
            context);
    }

    void audioDeviceAboutToStart(AudioIODevice* device) override
    {
        player.audioDeviceAboutToStart(device);
        display.audioDeviceAboutToStart(device);
    }

    void audioDeviceStopped() override
    {
        player.audioDeviceStopped();
        display.audioDeviceStopped();
    }

private:
    AudioSourcePlayer& player;
    LiveScrollingAudioDisplay& display;
};

//==============================================================================
//==============================================================================

class MidiProccessorSynth final : public Component,
    public juce::Button::Listener,
    public juce::ComboBox::Listener
{
public:
    MidiProccessorSynth()
    {
        // Boton para seleccionar el archivo midi
        addAndMakeVisible(buttonLoadMIDIFile = new juce::TextButton("Cargar archivo MIDI"));
        buttonLoadMIDIFile->addListener(this);

        // Combo box para seleccionar el track
        addAndMakeVisible(comboTrack = new juce::ComboBox());
        comboTrack->addListener(this);
        updateTrackComboBox();

        // Boton para reproducir el track seleccionado
        addAndMakeVisible(buttonPlayMIDIFile = new juce::TextButton("Reproducir archivo MIDI"));
        buttonPlayMIDIFile->addListener(this);

        synthAudioSource.setUsingSampledSound();
        addAndMakeVisible(liveAudioDisplayComp);

        setOpaque(true);
        setSize(650, 500);
    }

    ~MidiProccessorSynth() override
    {
        audioSourcePlayer.setSource(nullptr);
        audioDeviceManager.removeMidiInputDeviceCallback({}, &(synthAudioSource.midiCollector));
        audioDeviceManager.removeAudioCallback(&callback);
    }

    //==============================================================================
    
    void paint(Graphics& g) override
    {
        g.fillAll(getLookAndFeel().findColour(juce::ResizableWindow::backgroundColourId));
    }

    void resized() override
    {
        buttonLoadMIDIFile->setBounds(20, 20, 200, 50);
        comboTrack->setBounds(250, 20, 200, 50);
        buttonPlayMIDIFile->setBounds(20, 75, 200, 50);
        liveAudioDisplayComp.setBounds(10, 150, getWidth() - 20, getHeight() - 170);
    }

    //==============================================================================

    void buttonClicked(juce::Button* button)
    {
        if (button == buttonLoadMIDIFile)
        {
            FileChooser seleccion("Selecciona archivo MIDI", juce::File(), "*.mid*");

            if (seleccion.browseForFileToOpen())
            {
                synthAudioSource.readMidi(seleccion.getResult());
                updateTrackComboBox();
            }
        }
        if (button == buttonPlayMIDIFile)
        {
            synthAudioSource.processMidi();

            audioSourcePlayer.setSource(&synthAudioSource);
            audioDeviceManager.initialise(0, 2, nullptr, true);
            audioDeviceManager.addAudioCallback(&callback);
            audioDeviceManager.addMidiInputDeviceCallback({}, &(synthAudioSource.midiCollector));

        }
    }

    void comboBoxChanged(juce::ComboBox* combo)
    {
        if (combo == comboTrack)
        {
            synthAudioSource.currentTrack = combo->getSelectedId();
        }
    }

    void updateTrackComboBox()
    {
        comboTrack->clear();

        for (auto i = 1; i < synthAudioSource.numTracks; i++)
            comboTrack->addItem("Track " + juce::String(i), i);

        comboTrack->setSelectedId(synthAudioSource.currentTrack, juce::NotificationType::dontSendNotification);

    }

private:

    SynthAudioSource synthAudioSource{};
    AudioDeviceManager audioDeviceManager;
    AudioSourcePlayer audioSourcePlayer;
    LiveScrollingAudioDisplay liveAudioDisplayComp;
    Callback callback{ audioSourcePlayer, liveAudioDisplayComp };

    //==============================================================================
    juce::ScopedPointer<juce::TextButton> buttonLoadMIDIFile;
    juce::ScopedPointer<juce::TextButton> buttonPlayMIDIFile;
    juce::ScopedPointer<juce::ComboBox> comboTrack;

    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(MidiProccessorSynth)
};

