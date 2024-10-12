namespace alphappy.Archipelago
{
    internal static class Karma
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // When a story session starts, set the karma cap.
                On.StoryGameSession.ctor += StoryGameSession_ctor;
                // When a karma increase is received, update karma.
                //On.RainWorldGame.Update += RainWorldGame_Update;
            }

            private static void StoryGameSession_ctor(On.StoryGameSession.orig_ctor orig, StoryGameSession self, SlugcatStats.Name saveStateNumber, RainWorldGame game)
            {
                orig(self, saveStateNumber, game);
                if (Messenger.ArchiMode) self.UpdateKarma();
            }
        }

        internal static bool UpdateKarma(this StoryGameSession self)
        {
            var dpsd = self.saveState.deathPersistentSaveData;
            dpsd.karmaCap = Messenger.GameInbox.karmaCap;
            dpsd.karma = RWCustom.Custom.IntClamp(dpsd.karma, 0, dpsd.karmaCap);
            return true;
        }
    }
}
