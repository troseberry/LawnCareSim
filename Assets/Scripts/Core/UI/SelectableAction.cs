using System;

using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public enum SelectableActionType
    {
        ChangeColor,
        SwapSprite,
        SpriteSeries        // Should only be used with Pointer Down event type
    }

    [Serializable]
    public class SelectableAction
    {
        public SelectableActionType ActionType;
        public Graphic TargetGraphic;

        public Color DeselectedColor;
        public Color SelectedColor;

        private Image _targetGraphicImage;
        public Sprite DeselectedSprite;
        public Sprite SelectedSprite;
        public Sprite[] SpriteSeries;

        private int _currentSpriteIndex = 0;

        public void SetupTargetElements()
        {
            if (ActionType == SelectableActionType.SwapSprite || ActionType == SelectableActionType.SpriteSeries)
            {
                _targetGraphicImage = TargetGraphic.GetComponent<Image>();
            }
        }

        public void PerformSelection()
        {
            if (!TargetGraphic)
            {
                return;
            }

            switch (ActionType)
            {
                case SelectableActionType.ChangeColor:
                    if (SelectedColor != null)
                    {
                        TargetGraphic.color = SelectedColor;
                    }
                    break;
                case SelectableActionType.SwapSprite:
                    if (SelectedSprite != null && _targetGraphicImage)
                    {
                        _targetGraphicImage.sprite = SelectedSprite;
                    }
                    break;
                case SelectableActionType.SpriteSeries:
                    if (SpriteSeries == null || SpriteSeries.Length == 0)
                    {
                        return;
                    }

                    int nextSprite = _currentSpriteIndex + 1 == SpriteSeries.Length ? 0 : _currentSpriteIndex + 1;
                    if (SpriteSeries[nextSprite] != null && _targetGraphicImage)
                    {
                        _targetGraphicImage.sprite = SpriteSeries[nextSprite];
                        _currentSpriteIndex = nextSprite;
                    }
                    break;
                default:
                    Debug.LogWarning("SelectableAction has invalid action type");
                    break;
            }
        }

        public void PerformDeselection()
        {
            if (!TargetGraphic)
            {
                return;
            }

            switch (ActionType)
            {
                case SelectableActionType.ChangeColor:
                    if (DeselectedColor != null)
                    {
                        TargetGraphic.color = DeselectedColor;
                    }
                    break;
                case SelectableActionType.SwapSprite:
                    if (DeselectedSprite != null && _targetGraphicImage)
                    {
                        _targetGraphicImage.sprite = DeselectedSprite;
                    }
                    break;
                case SelectableActionType.SpriteSeries:
                    if (SpriteSeries == null || SpriteSeries.Length == 0)
                    {
                        return;
                    }

                    int nextSprite = _currentSpriteIndex + 1 == SpriteSeries.Length ? 0 : _currentSpriteIndex + 1;
                    if (SpriteSeries[nextSprite] != null && _targetGraphicImage)
                    {
                        _targetGraphicImage.sprite = SpriteSeries[nextSprite];
                        _currentSpriteIndex = nextSprite;
                    }
                    break;
                default:
                    Debug.LogWarning("SelectableAction has invalid action type");
                    break;
            }
        }
    }
}