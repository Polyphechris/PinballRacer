using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace PinballRacer
{
    public enum AudioEffect { WALL_BOUNCE, TIRE_BOUNCE, BUMPER_BOUNCE, FLIPPER_BOUNCE };
    public enum Music { MAIN_MENU, RACE, CREDITS };

    // Class to hold the sound effects
    public class AudioEffectsDictionary : Dictionary<AudioEffect, SoundEffect>
        {
            public object Value { get; set; }
 
            public override string ToString()
            {
                return (Value ?? string.Empty).ToString();
            }
        }

    //Class to hold the music
    public class MusicDictionary : Dictionary<Music, Song>
        {
            public object Value { get; set; }
 
            public override string ToString()
            {
                return (Value ?? string.Empty).ToString();
            }
        }

    class AudioManager
    {
        private static MusicDictionary songs;
        private static AudioEffectsDictionary effects;
        private const float volumeModifier = 0.05f;

        public AudioManager(MusicDictionary newSongs, AudioEffectsDictionary newEffects)
        {
            MediaPlayer.Volume = 0.5f;
            songs = newSongs;
            effects = newEffects;
        }

        // Play an effect
        public static void playEffect(AudioEffect e)
        {
            if (effects.Keys.Contains(e))
            {
                effects[e].Play();
            }
        }

        // Stops the current music and plays another song
        public static void playMusic(Music m)
        {
            if (songs.Keys.Contains(m))
            {
                stopMusic();
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(songs[m]);
            }
        }

        // Fades then stops the music
        public static void stopMusic()
        {
            FadeMusicVolume(0);
            MediaPlayer.Stop();
            RaiseMusicVolume(0.5f);
        }

        // Pauses the music
        public static void pauseMusic()
        {
            MediaPlayer.Pause();
        }

        // Fades the music to a certain level
        public static void FadeMusicVolume(float newVolume)
        {
            // If the new volume is valid, perform the fade
            if (newVolume >= 0)
            {
                while (MediaPlayer.Volume > newVolume)
                {
                    MediaPlayer.Volume -= volumeModifier;
                }
            }
        }

        // Raises the music volume to a certain level
        public static void RaiseMusicVolume(float newVolume)
        {
            // If the new volume is valid, perform the raise
            if (newVolume <= 1)
            {
                while (MediaPlayer.Volume < newVolume)
                {
                    MediaPlayer.Volume += volumeModifier;
                }
            }
        }
    }
}
