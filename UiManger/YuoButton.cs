using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace YuoTools
{
    public class YuoButton : Button
    {
        public override void Select()
        {
            base.Select();
            "select".Log();
        }
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            "OnSubmit".Log();
        }
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            "OnSelect".Log();
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            "OnPointerEnter".Log();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            "OnRectTransformDimensionsChange".Log();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            base.OnBeforeTransformParentChanged();
            "OnBeforeTransformParentChanged".Log();
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            "OnCanvasHierarchyChanged".Log();
        }

        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();
            "OnCanvasGroupChanged".Log();
        }

        public override bool IsInteractable()
        {
            "IsInteractable".Log();
            return base.IsInteractable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();
            "OnDidApplyAnimationProperties".Log();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            "OnEnable".Log();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            "OnTransformParentChanged".Log();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            "OnDisable".Log();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            "OnValidate".Log();
        }

        protected override void Reset()
        {
            base.Reset();
            "Reset".Log();
        }

        protected override void InstantClearState()
        {
            base.InstantClearState();
            "InstantClearState".Log();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            "DoStateTransition".Log();
        }

        public override Selectable FindSelectableOnLeft()
        {
            "FindSelectableOnLeft".Log();
            return base.FindSelectableOnLeft();
        }

        public override Selectable FindSelectableOnRight()
        {
            "FindSelectableOnRight".Log();
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp()
        {
            "FindSelectableOnUp".Log();
            return base.FindSelectableOnUp();
        }

        public override Selectable FindSelectableOnDown()
        {
            "FindSelectableOnDown".Log();
            return base.FindSelectableOnDown();
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            "OnMove".Log();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            "OnPointerDown".Log();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            "OnPointerUp".Log();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            "OnPointerExit".Log();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            "OnDeselect".Log();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            "OnPointerClick".Log();
        }
    }
}
