using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Entities.Singletons
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Ed Linero</Owner>
    public class SoundManager
    {

        Stack<Event> events;

        AudioEngine engine;
        SoundBank soundBank;
        WaveBank waveBank;
        List<Song> soundList;
        Song[] soundArray;
        protected Song invasion;
        protected Song roughThemeSmall;
        protected Song ouch;
        protected Song pickup;
        protected Song playerDamage;
        protected Song squeal;

        String[] strArry;
        List<String> nameList;
       // public delegate void ResponseMethod(int i);
       // private ResponseMethod soundPtr;
        


        //used for singleton. 
        private static SoundManager myManager;

        public static SoundManager createSoundManager(ContentManager cont)
        {
            if (myManager == null)
            {
                myManager = new SoundManager(cont);
            }
            
            return myManager;
        }


        private SoundManager(ContentManager cont)
        {


            engine = new AudioEngine(@"..\..\..\..\CSE308GameContent\Sounds\XACT\Win\community.xgs");
            soundBank = new SoundBank(engine, @"..\..\..\..\CSE308GameContent\Sounds\XACT\Win\sounds.xsb");
            waveBank = new WaveBank(engine, @"..\..\..\..\CSE308GameContent\Sounds\XACT\Win\waves.xwb");
            soundList = new List<Song>();
            nameList = new List<String>();
            storeSounds(cont);
            events = new Stack<Event>();
        }


        public void storeSounds(ContentManager cont)
        {


            //Cue[] toReturn = null;
            
            //Cue soundEffect;


            invasion = cont.Load<Song>("MusicSfx\\invasion");
            roughThemeSmall = cont.Load<Song>("MusicSfx\\roughThemeSmall");
            //playerDamage = cont.Load<Song>("MusicSfx\\playerDamage");
            //squeal = cont.Load<Song>("MusicSfx\\squeal");
            //pickup = cont.Load<Song>("MusicSfx\\pickup");
            //ouch = cont.Load<Song>("MusicSfx\\ouch");


            nameList.Add("invasion");
            nameList.Add("roughThemeSmall");
            nameList.Add("playerDamage");
            nameList.Add("squeal");
            nameList.Add("pickup");
            nameList.Add("ouch");

            soundList.Add(invasion);
            soundList.Add(roughThemeSmall);
            soundList.Add(playerDamage);
            soundList.Add(squeal);
            soundList.Add(pickup);
            soundList.Add(ouch);
            
            

            strArry = nameList.ToArray();
            soundArray = soundList.ToArray();

            /*for (int i = 0; i < nameList.Count; i++)
            {
                soundEffect = soundBank.GetCue(strArry[i]);
                soundList.Add(soundEffect);
            }

            toReturn = soundList.ToArray();

            return toReturn;*/
            
        }

        private void ControlSound(String fxName, String state)
        {

            Song soundEffect = null;
            Cue soundEffectCue = null;
            
            
            if (strArry != null)
            {
                //String check = "";
                //int index = -1;

             
                    for(int i = 0; i < strArry.Length ; i++)
                    {
                        
                        if(strArry[i].Contains(fxName))
                        {

                            if (fxName.Equals("invasion") || fxName.Equals("roughThemeSmall"))
                            {

                                //index = j;
                                soundEffect = soundArray.ElementAt<Song>(i);

                                if (state.Equals(EventList.PlaySound))
                                {
                                    MediaPlayer.Play(soundEffect);
                                }
                                else if (state.Equals(EventList.PauseSound))// && soundEffect.IsPlaying)
                                {

                                    MediaPlayer.Pause();
                                }

                                else if (state.Equals(EventList.StopSound))// && soundEffect.IsPlaying)
                                {

                                    MediaPlayer.Stop();

                                }
                            }
                            else
                            {
                                soundEffectCue = soundBank.GetCue(strArry[i]);

                                if (state.Equals(EventList.PlaySound))
                                {
                                    soundEffectCue.Play();
                                }
                                else if (state.Equals(EventList.PauseSound) && soundEffectCue.IsPlaying)
                                {

                                    soundEffectCue.Pause();
                                }

                                else if (state.Equals(EventList.StopSound) && soundEffectCue.IsPlaying)
                                {

                                    soundEffectCue.Stop(AudioStopOptions.AsAuthored);

                                }
                            }
                            
                        }
                    
                  
                    }
               
            }
        }

        public void addEvent(Event e)
        {
            events.Push(e);
        }

        public void handleEvents()
        {
            while (events.Count > 0)
            {
                Event e = events.Pop();
                ControlSound((e._value as String),e.type);
            }
        }

    }
}
