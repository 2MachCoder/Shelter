using System.Collections.Generic;
using Core.Systems;
using Core.Systems.DataPersistenceSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameScripts.PlayerScripts;
using Interfaces;
using UIModules.GameScreen.Scripts;
using UnityEngine;
using Zenject;

namespace GameScripts
{
    public class GameplayModule : MonoBehaviour, IDataPersistence
    {
        [Inject] private Camera _camera;
        [Inject] private AudioSystem _audioSystem;
        
        [SerializeField] private GameObject levelWalls;
        [SerializeField] private Core.AbstractClasses.OpenClosable blastDoor;
        [SerializeField] private Core.AbstractClasses.OpenClosable blastHatch;
        [SerializeField] private Core.AbstractClasses.Interactable FVM;  // Filter ventilation machine
        //[SerializeField] private float breakingTime = 20000f;
        //[SerializeField] private float breakTimeMultiplier = 1.2f;
        //public LevelScriptableObject[] levelScriptableObjects;
        //private LevelScriptableObject _currentLevelScriptableObject;
        //private List<Breakable> _breakables = new();  
        [SerializeField] private float messageFadeTime = 0.5f;
        public Player player;
        private GameScreenUIView _gameScreenUIView;
        private IBreakable _currentBroken;
        private bool _surviveStarted;
        private bool _allIsFine = true;

        public void Initialize(GameScreenUIView gameScreenUIView)
        {
            _gameScreenUIView = gameScreenUIView;
            player = Instantiate(player, transform).GetComponent<Player>();
            player.Initialize(gameScreenUIView, _camera);
        }

        public async void StartGame()
        {
            await UniTask.Delay(1000);
            var messages = new Dictionary<string, int>
            {
                { "That's was a typical day", 3000},
                { "But...", 1000*2},
            }; 
            await ShowGameMessage(messages);
            //Sound of siren on, timer on
            await UniTask.Delay(1000 * 60); //1 minute
            //DetonateBomb();
        }

        private void GenerateLevel()
        {
            player.transform.localPosition = new Vector3(-0.3f, 5f, -5f);
            levelWalls = Instantiate(levelWalls, transform);
            blastDoor = Instantiate(blastDoor, transform);
            blastDoor.transform.position = new Vector3(0f, 0.482f, 0f);
            blastHatch = Instantiate(blastHatch, transform);
            blastHatch.transform.position = new Vector3(2f, 0.482f, 0f);
            FVM = Instantiate(FVM, transform);
            FVM.transform.position = new Vector3(-2f, 0.482f, 0f);
        }

        // private void DetonateBomb()
        // {
        //
        // }

        private void OnPlayerInsideShelter()
        {
            var messages = new Dictionary<string, int>()
            {
                { "Welcome to the shelter", 2000},
                { "Now you need to close all the doors", 2500},
                //  Add task CloseDoors
            };
            ShowGameMessage(messages).Forget();
        }

        private async void BreakSomething()
        {
            // while (player.MoveModule.IsAlive)
            // {
            //     int index = Random.Range(0, breakable.Count); 
            //     //breakables[index]Break();
            //     await UniTask.Delay((int)(breakingTime * breakTimeMultiplier));
            // }
        }
        
        private async UniTask ShowGameMessage(Dictionary<string, int> messages)
        {
            foreach (var message in messages)
            {
                _gameScreenUIView.messageText.gameObject.SetActive(true);
                _gameScreenUIView.messageText.text = message.Key;
                _gameScreenUIView.messageText.DOColor(Color.white, 0); 
                await _gameScreenUIView.messageText.DOFade(1f, messageFadeTime);
                await UniTask.Delay(message.Value);
                await _gameScreenUIView.messageText.DOFade(0f, messageFadeTime);  
                _gameScreenUIView.messageText.gameObject.SetActive(false);
            }
        }
        
        private async UniTask ShowGameMessage(string message, int time)
        {
            _gameScreenUIView.messageText.gameObject.SetActive(true);
            _gameScreenUIView.messageText.text = message;
            _gameScreenUIView.messageText.DOColor(Color.white, 0); 
            await _gameScreenUIView.messageText.DOFade(1f, messageFadeTime);
            await UniTask.Delay(time);
            await _gameScreenUIView.messageText.DOFade(0f, messageFadeTime);  
            _gameScreenUIView.messageText.gameObject.SetActive(false);
        }

        public void Show()
        {
            GenerateLevel();
            player.MoveModule.OnPlayerInsideShelter += OnPlayerInsideShelter;
        }

        public void Hide()
        {
            player.MoveModule.OnPlayerInsideShelter -= OnPlayerInsideShelter;
            player.Hide();
            Destroy(player);
            Destroy(blastDoor);
            Destroy(levelWalls);
        }
        
        public void LoadData(GameData gameData)
        {
            //Not ready yet
        }

        public void SaveData(ref GameData gameData)
        {
            //Not ready yet
        }
    }
}
