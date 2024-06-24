using Core.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Utility
{
    public static class UIHelpers
    {
        public enum UINavigationDirection
        {
            Vertical,
            Horizontal,
            Grid
        }

        #region Setup UI Elements
        public static void SetUpUIElement(Transform parentTransform, ref TextMeshProUGUI uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            TextMeshProUGUI componentResult = result.GetComponent<TextMeshProUGUI>();
            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        public static void SetUpUIElement(Transform parentTransform, ref RectTransform uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            RectTransform componentResult = result.GetComponent<RectTransform>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        public static void SetUpUIElement(Transform parentTransform, ref Image uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            Image componentResult = result.GetComponent<Image>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        public static void SetUpUIElement(Transform parentTransform, ref Button uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            Button componentResult = result.GetComponent<Button>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<Button>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        public static void SetUpUIElement(Transform parentTransform, ref Slider uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            Slider componentResult = result.GetComponent<Slider>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<Slider>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        public static void SetUpUIElement(Transform parentTransform, ref Selectable uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            Selectable componentResult = result.GetComponent<Selectable>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<Selectable>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        
        public static void SetUpUIElement(Transform parentTransform, ref RBSelectable uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            RBSelectable componentResult = result.GetComponent<RBSelectable>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<RBSelectable>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find RBSelectable element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }

        public static void SetUpUIElement(Transform parentTransform, ref TMP_Dropdown uiElement, string elementName)
        {
            Transform result = parentTransform.FindDeepChild(elementName);
            TMP_Dropdown componentResult = result.GetComponent<TMP_Dropdown>();

            if (result != null && componentResult != null)
            {
                uiElement = result.GetComponent<TMP_Dropdown>();
            }
            else
            {
                Debug.LogWarning($"[UIHelpers] [SetUpCanvasElement] - Could not find TMP_Dropdown element of name: {elementName}. (Transform null? {result == null}) | (Component null? {componentResult == null})");
            }
        }
        #endregion

        #region SetUpNavigation for Instantiated UI Elements
        public static void SetupExplicitNavigation(UINavigationDirection dir, ref List<GameObject> selectableObjects)
        {
            for (int i = 0; i < selectableObjects.Count; i++)
            {
                Navigation navStruct = new Navigation() { mode = Navigation.Mode.Explicit };

                // Previous Selection
                if (i > 0)
                {
                    if (dir == UINavigationDirection.Vertical)
                    {
                        navStruct.selectOnUp = selectableObjects[i - 1].GetComponent<Selectable>();
                    }
                    else if (dir == UINavigationDirection.Horizontal)
                    {
                        navStruct.selectOnLeft = selectableObjects[i - 1].GetComponent<Selectable>();
                    }
                }

                // Next Selection
                if (i < selectableObjects.Count - 1)
                {
                    if (dir == UINavigationDirection.Vertical)
                    {
                        navStruct.selectOnDown = selectableObjects[i + 1].GetComponent<Selectable>();
                    }
                    else if (dir == UINavigationDirection.Horizontal)
                    {
                        navStruct.selectOnRight = selectableObjects[i + 1].GetComponent<Selectable>();
                    }
                }

                selectableObjects[i].GetComponent<Selectable>().navigation = navStruct;
            }
        }

        public static void SetupExplicitNavigation(UINavigationDirection dir, ref List<GameObject> selectableObjects, int gridRowLength, bool allowWrapAround = false)
        {
            if (gridRowLength <= 0)
            {
                Debug.LogWarning($"Can't setup explicit navigation for items. Grid Row Length is invalid");
                return;
            }

            //-1 is null selectable. want this empty instead of this index so no extra selection events fire
            int leftSelection = -1;
            int rightSelection = -1;
            int upSelection = -1;
            int downSelction = -1;

            for (int i = 0; i < selectableObjects.Count; i++)
            {
                #region Calculate Indexes
                int lastIndex = selectableObjects.Count - 1;

                //Standard indexes
                leftSelection = i + -1;
                rightSelection = i + 1;
                upSelection = i - gridRowLength;
                downSelction = i + gridRowLength;

                // First in row
                if ((i + gridRowLength) % gridRowLength == 0)
                {
                    leftSelection = -1;
                    if (allowWrapAround)
                    {
                        leftSelection = i + (gridRowLength - 1);
                    }
                }

                // Last in row
                if ((i % gridRowLength) == (gridRowLength - 1))
                {
                    rightSelection = -1;
                    if (allowWrapAround)
                    {
                        rightSelection = i - (gridRowLength - 1);
                    }
                }

                // First in column
                if (i < gridRowLength)
                {
                    upSelection = -1;
                    if (allowWrapAround)
                    {
                        int lastIndexColumnPosition = lastIndex % gridRowLength;
                        int itemColumnPosition = i % gridRowLength;

                        int distanceBetweenPositions = lastIndexColumnPosition - itemColumnPosition;

                        //zero distance
                        if (distanceBetweenPositions == 0)
                        {
                            //wrap around index is last index;
                            upSelection = lastIndex;
                        }
                        //positive distance
                        else if (distanceBetweenPositions > 0)
                        {
                            upSelection = lastIndex - distanceBetweenPositions;
                        }
                        //negative distance - wrap around index is higher column that last index (i.e. to the right of)
                        else if (distanceBetweenPositions < 0)
                        {
                            upSelection = lastIndex - gridRowLength + (distanceBetweenPositions * -1);
                        }
                    }
                }

                // Last in column
                if (i > (selectableObjects.Count - gridRowLength - 1))
                {
                    downSelction = -1;
                    if (allowWrapAround)
                    {
                        int itemColumnPosition = i % gridRowLength;
                        downSelction = 0 + itemColumnPosition;
                    }
                }

                // Last in list
                if (i == selectableObjects.Count - 1)
                {
                    rightSelection = -1;
                    if (allowWrapAround)
                    {
                        leftSelection = -1;

                        int itemColumPosition = i % gridRowLength;
                        if (itemColumPosition > 0)
                        {
                            rightSelection = i - itemColumPosition;
                        }
                    }
                }

                // Index Out of Range Protection
                leftSelection = leftSelection > lastIndex ? -1 : leftSelection;
                rightSelection = rightSelection > lastIndex ? -1 : rightSelection;
                upSelection = upSelection > lastIndex ? -1 : upSelection;
                downSelction = downSelction > lastIndex ? -1 : downSelction;
                #endregion

                Navigation navStruct = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = leftSelection > -1 ? selectableObjects[leftSelection].GetComponent<Selectable>() : null,
                    selectOnRight = rightSelection > -1 ? selectableObjects[rightSelection].GetComponent<Selectable>() : null,
                    selectOnUp = upSelection > -1 ? selectableObjects[upSelection].GetComponent<Selectable>() : null,
                    selectOnDown = downSelction > -1 ? selectableObjects[downSelction].GetComponent<Selectable>() : null
                };


                selectableObjects[i].GetComponent<Selectable>().navigation = navStruct;
            }
        }
        #endregion
    }
}
