using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Random = System.Random;

namespace Edgar.Unity.Examples.Gungeon
{
    /// <summary>
    /// Example of a simple game manager that uses the DungeonGeneratorRunner to generate levels.
    /// </summary>
    public class GungeonGameManager : GameManagerBase<GungeonGameManager>
    {
        // Current active room

        private RoomInstanceGrid2D currentRoom;

        // The room that will be active after the player leaves the current room
        private RoomInstanceGrid2D nextCurrentRoom;

        private long generatorElapsedMilliseconds;

        // To make sure that we do not start the generator multiple times
        protected bool isGenerating = false;

        // Shared instance of the random numbers generator
        public Random Random;

        [Range(1, 5)]
        public int Stage = 1;

        public LevelGraph CurrentLevelGraph;

        /*[Header("Post-processing tasks for each stage")]
        public GungeonPostProcessingTask Stage1PostProcessing;
        public GungeonPostProcessingTask Stage3PostProcessing;
        public GungeonPostProcessingTask Stage4PostProcessing;*/

        public void Update() {
            if (InputHelper.GetKeyDown(KeyCode.G) && isGenerating) {
                Stage += 1;
                LoadNextLevel();  // G는 같은 스테이지에서 다시 제네레이트
            }

            /*
            if (InputHelper.GetKeyDown(KeyCode.H) && isGenerating)
            {
                // H는 다음 스테이지로 이동.
                LoadNextLevel();
            }
            */
        }

        /*private GungeonPostProcessingTask GetPostProcessingTaskForStage() {
            switch (Stage) {
                case 1: return Stage1PostProcessing;
                case 2: return Stage2PostProcessing;
                case 3: return Stage3PostProcessing;
                case 4: return Stage4PostProcessing;
                default: return Stage1PostProcessing; // 기본적으로 스테이지 1을 반환
            }
        }*/

        public void SetIsGenerating(bool value) {
            isGenerating = value;
        }

        public override void LoadNextLevel() {
            if (Stage > 1)
                ClearPooledItems();
            isGenerating = true;
            // Show loading screen
            ShowLoadingScreen("Lucid Dream", $"Stage {Stage}");

            // Find the generator runner
            var generator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorGrid2D>();
            //generator.PostProcessingTask = GetPostProcessingTaskForStage();
            // Start the generator coroutine
            StartCoroutine(GeneratorCoroutine(generator));
        }

        /// <summary>
        /// Coroutine that generates the level.
        /// We need to yield return before the generator starts because we want to show the loading screen
        /// and it cannot happen in the same frame.
        /// It is also sometimes useful to yield return before we hide the loading screen to make sure that
        /// all the scripts that were possibly created during the process are properly initialized.
        /// </summary>
        private IEnumerator GeneratorCoroutine(DungeonGeneratorGrid2D generator) {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            // Configure the generator with the current stage number
            var inputTask = (GungeonInputSetupTask)generator.CustomInputTask;
            inputTask.Stage = Stage;

            var generatorCoroutine = this.StartSmartCoroutine(generator.GenerateCoroutine());

            yield return generatorCoroutine.Coroutine;

            yield return null;

            stopwatch.Stop();

            isGenerating = false;

            // Throw an exception if the coroutine was not successful.
            // The point of this custom coroutine is that you can actually catch the exception (unlike with the default coroutines).
            // It makes it possible to run the generator again if needed while still having coroutines and not blocking the main thread.
            generatorCoroutine.ThrowIfNotSuccessful();

            generatorElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            RefreshLevelInfo();
            HideLoadingScreen();
        }

        private void RefreshLevelInfo() {
            var info = $"Generated in {generatorElapsedMilliseconds / 1000d:F}s\n";
            info += $"Stage: {Stage}, Level graph: {CurrentLevelGraph.name}\n";
            info += $"Room type: {(currentRoom?.Room as GungeonRoom)?.Type}, Room template: {currentRoom?.RoomTemplatePrefab.name}";

            SetLevelInfo(info);
        }

        public void OnRoomEnter(RoomInstanceGrid2D roomInstance) {
            nextCurrentRoom = roomInstance;

            if (currentRoom == null) {
                currentRoom = nextCurrentRoom;
                nextCurrentRoom = null;
                RefreshLevelInfo();
            }
        }

        public void OnRoomLeave(RoomInstanceGrid2D roomInstance) {
            currentRoom = nextCurrentRoom;
            nextCurrentRoom = null;
            RefreshLevelInfo();
        }

        private void ClearPooledItems() {
            foreach (var obj in GameObject.FindGameObjectsWithTag("PooledItem")) {
                obj.SetActive(false);
            }
        }
    }
}