using TMPro;
using UnityEngine;
namespace RomanDoliba.Board
{
    public sealed class ScoreCounter : MonoBehaviour
    {
        public static ScoreCounter Instance {get; private set;}
        [SerializeField] private TextMeshProUGUI _scoreText;
        private static int _score;
        public int Score
        {
            get => _score;
            set
            {
                if (_score == value) return;
                _score = value;
                _scoreText.SetText($"Score = {_score}");
            }

        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
