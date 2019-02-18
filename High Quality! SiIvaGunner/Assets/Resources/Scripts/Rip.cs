using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Rip : MonoBehaviour {
    //private readonly List<string> LANES = new List<string>(new string[] { "L3", "L2", "L1", "C", "R1", "R2", "R3" });
    private readonly float[] ACCURACY = { 0f, 0.3f, 0.5f, 0.7f, 0.9f, 1f };

    //WANO 1.331 200 //LL 0.342 212 
    public float sfxVol { get; set; }

    private AudioSource source;
    private AudioClip rip;
    private float offset;
    private float bpm;
    private float moveSpeed;
    private int sampleDelay = 0;
    private bool isPaused;
    private int spb;  //samples per beat
    private int destroyDelay;
    private Queue<NoteData> notes = new Queue<NoteData>();
    private List<Note> onScreen = new List<Note>();

    private long score;
    private long combo;
    private long maxCombo;
    private long[] qualityList = new long[6];

    /** Constructs a new HighQualityRip containing the music data. **/
    void Start() {
        sfxVol = 1;
        source = GetComponent<AudioSource>();
        init();
        playSong();
    }

    /** Initializes values within the rip, note, and HeldNoteUp classes. **/
    private void init() {
        SongData songdata = Scenes.getSongData();
        rip = songdata.rip;
        offset = songdata.offset;
        bpm = songdata.bpm;
        moveSpeed = songdata.moveSpeed;

        source.clip = rip;
        spb = secondsToSamples(60 / bpm);
        destroyDelay = spb * 16; //lul
        moveSpeed /= rip.frequency;

        Note.initAll(this, destroyDelay);
        HeldNoteUp.initAll(this, destroyDelay);

        if (songdata.beatmap == null) generateRandomValues();
        else foreach (string note in songdata.beatmap) notes.Enqueue(new NoteData(note));
    }

    /** Plays the song giving it a 1 second offset. **/
    private void playSong() {
        float songDelay = 1 - offset;
        if (songDelay > 0) {
            sampleDelay = secondsToSamples(songDelay);
            int offsetSamp = sampleDelay * rip.channels;
            float[] origSamples = new float[rip.samples * rip.channels];
            rip.GetData(origSamples, 0);
            float[] samples = new float[(rip.samples + sampleDelay) * rip.channels];
            source.clip = AudioClip.Create(rip.name, rip.samples + sampleDelay, rip.channels, rip.frequency, false);
            for (int i = offsetSamp; i < samples.Length; i++) samples[i] = origSamples[i - offsetSamp];
            rip = source.clip;
            rip.SetData(samples, 0);
        }
        //else source.time = -songDelay;  //kEEP because this is the offset from SONG START not note start
        else source.time = 0;
        source.Play();
    }

    void Update() {
        //spawns the notes
        while (notes.Count > 0 && source.timeSamples >= notes.Peek().timePosition) onScreen.Add(Note.Create(notes.Dequeue()));

        //adds hittability
        bool[] rows = new bool[7];
        bool allRows = false;
        for (int i = 0; i < onScreen.Count && !allRows; i++) {
            if (!rows[onScreen[i].getLane()]) {
                onScreen[i].setInteractable(true);
                rows[onScreen[i].getLane()] = true;
            }
            allRows = true;
            for (int j = 0; j < rows.Length; j++) allRows = rows[j] && allRows;
        }

        //loads the end game screen once the player finishes the song
        if (!source.isPlaying && source.time == 0) {
            StartCoroutine(endGame());
        }
        //if (something) gameOver();
    }

    /** Loads the end game screeon when the player successfully finishes the rip. **/
    private IEnumerator endGame() {
        //something something full combo animation
        //something something gotta load the records
        //something something fade to black animation
        yield return new WaitForSeconds(samplesToSeconds(spb * 4) + 2);
        Scenes.Load("EndCard", new PlayData(score, maxCombo, qualityList));
    }

    /** Loads the game over screen when the player loses all their health. **/
    private void gameOver() {
        //just a window the size of "just monika" lmao
        //probably just a msg "haha ur a scrub" and then
        //retry abort fail
    }

    /** Generates a random beatmap. **/
    private void generateRandomValues() {
        int[] isFilled = new int[7];
        for (int i = 0; i < isFilled.Length; i++) isFilled[i] = secondsToSamples(offset) - destroyDelay - 777;
        int mid = UnityEngine.Random.Range(3, 5);
        for (int time = secondsToSamples(offset) - destroyDelay; time <= rip.samples - destroyDelay; time += spb) {
            if (!(UnityEngine.Random.value > 0.77f && generateNote(isFilled, time, 0, 7))) {
                bool filled = false;
                for (int i = 0; i < 7; i++) if (isFilled[i] >= time) filled = true;
                if (!filled) mid = UnityEngine.Random.Range(3, 5);

                generateNote(isFilled, time, 0, mid);
                generateNote(isFilled, time, mid, 7);
            }
        }
    }

    /** Generates a random note. **/
    private bool generateNote(int[] isFilled, int time, int start, int end) {
        NoteData nd;
        int lane = UnityEngine.Random.Range(start - 3, end - 3) * 2;
        if (isFilled[lane / 2 + 3] < time) {
            bool filled = false;
            for (int i = start; i < end; i++) if (isFilled[i] >= time) filled = true;
            if (!filled) {
                int heldFor = UnityEngine.Random.value >= 0.77f ? spb * UnityEngine.Random.Range(1, 4) : 0;
                nd = new NoteData(time, lane, moveSpeed, heldFor != 0, heldFor);
                if (time + heldFor <= rip.samples - destroyDelay) {
                    isFilled[lane / 2 + 3] = time + heldFor;
                    notes.Enqueue(nd);
                    return true;
                }
            }
        }
        return false;
    }

    /** 
     * Calculates and updates the score when the player taps a note or when a note misses.
     * @param q     The quality of the tap.
     * @param n     The note to be removed from the onScreen list.
     **/
    public void updateScore(int q, Note n) {
        qualityList[q]++;
        combo = q > 2 ? combo + 1 : q > 0 ? 1 : 0;
        if (combo > maxCombo) maxCombo = combo;

        //TODO change the scoring system
        float tempScore = 777 * Mathf.Min(1 + (float)combo / 777, 2) * ACCURACY[q];
        if (n.isHold()) tempScore *= 1.25f;
        //TODO change the scoring system

        score += (int)tempScore;
        gameObject.GetComponent<TextAnimation>().updateScore(q, score, combo);
        if (q == 0) gameObject.GetComponent<HealthBar>().addHealth(-14);
        if (q == 1) gameObject.GetComponent<HealthBar>().addHealth(-7);
        onScreen.Remove(n);
    }

    /** @return The current score. **/
    public long getScore() {
        return score;
    }

    /** @return The current time samples. **/
    public int getSamples() {
        return source.timeSamples - sampleDelay;
    }

    /** @return The current time samples. **/
    public int getTotalSamples() {
        return rip.samples - sampleDelay;
    }

    /** 
     * @param  The seconds.
     * @return The number of samples equivalent to the seconds. **/
    public int secondsToSamples(float seconds) {
        return (int)(seconds * rip.frequency);
    }
    /** 
     * @param  The samples.
     * @return The number of seconds equivalent to the samples. **/
    public float samplesToSeconds(int samples) {
        return (float)samples / rip.frequency;
    }

    /** @return Whether or not the game is paused. **/
    public bool getPause() {
        return isPaused;
    }
    /** @param pause    The next state of the game. **/
    public IEnumerator setPause(bool pause) {
        if (pause && !isPaused) {
            source.Pause();
            isPaused = true;
        }
        else {
            yield return new WaitForSeconds(samplesToSeconds(spb * 3));
            isPaused = false;
            source.UnPause();
        }
    }

    /** @return The bpm. **/
    public float getMoveSpeed() {
        return moveSpeed;
    }
    /** @param pos      Plays the hit sound effect at the given position. **/
    public void playHit(Vector3 pos) {
        AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("ding"), new Vector3(pos.x, -3, pos.z), sfxVol);
    }
}

/** Holds the data for a note before it is initialized. **/
public class NoteData {
    public int timePosition;
    public Vector3 lane;
    public float moveSpeed;
    public bool isHeld;
    public int heldFor;
    public NoteData(int tp, int l, float mS, bool iH, int hF) {
        timePosition = tp;
        lane = new Vector3(l, 100, -1);
        moveSpeed = mS;
        isHeld = iH;
        heldFor = hF;
    }
    public NoteData(String beat) {
        //file stuff
    }
}