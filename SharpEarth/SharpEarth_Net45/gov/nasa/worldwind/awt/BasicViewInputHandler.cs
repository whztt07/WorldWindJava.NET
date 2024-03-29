/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.awt.event;
using java.awt;
using SharpEarth.view.ViewUtil;
using SharpEarth.geom;
using SharpEarth;
namespace SharpEarth.awt{



/**
 * @author dcollins
 * @version $Id: BasicViewInputHandler.java 2251 2014-08-21 21:17:46Z dcollins $
 */
public abstract class BasicViewInputHandler : AbstractViewInputHandler
{
    protected abstract void onMoveTo(Position focalPosition,
        ViewInputAttributes.DeviceAttributes deviceAttributes,
        ViewInputAttributes.ActionAttributes actionAttribs);

    protected abstract void onHorizontalTranslateRel(double forwardInput, double sideInput,
        double sideInputFromMouseDown, double forwardInputFromMouseDown,
        ViewInputAttributes.DeviceAttributes deviceAttributes,
        ViewInputAttributes.ActionAttributes actionAttributes);

    protected abstract void onVerticalTranslate(double translateChange, double totalTranslateChange,
        ViewInputAttributes.DeviceAttributes deviceAttributes,
        ViewInputAttributes.ActionAttributes actionAttributes);

    protected abstract void onRotateView(double headingInput, double pitchInput,
        double totalHeadingInput, double totalPitchInput,
        ViewInputAttributes.DeviceAttributes deviceAttributes,
        ViewInputAttributes.ActionAttributes actionAttributes);

    protected abstract void onResetHeading(ViewInputAttributes.ActionAttributes actionAttribs);

    protected abstract void onResetHeadingPitchRoll(ViewInputAttributes.ActionAttributes actionAttribs);

    public class RotateActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler, KeyEventState keys, String target,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            java.util.List keyList = viewAction.getKeyActions();
            double headingInput = 0;
            double pitchInput = 0;
            foreach (Object k in keyList) {
                ViewInputAttributes.ActionAttributes.KeyAction keyAction =
                    (ViewInputAttributes.ActionAttributes.KeyAction) k;
                if (keys.isKeyDown(keyAction.keyCode))
                {
                    if (keyAction.direction == ViewInputAttributes.ActionAttributes.KeyAction.KA_DIR_X)
                    {
                        headingInput += keyAction.sign;
                    }
                    else
                    {
                        pitchInput += keyAction.sign;
                    }

                }
            }
            
            if (headingInput == 0 && pitchInput == 0)
            {
                return false;
            }

            //noinspection StringEquality
            if (target == GENERATE_EVENTS)
            {

                ViewInputAttributes.DeviceAttributes deviceAttributes =
                    inputHandler.getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_KEYBOARD);

                onRotateView(headingInput, pitchInput, headingInput, pitchInput, deviceAttributes, viewAction);

            }
            return true;
        }
    }

    public class HorizontalTransActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler, KeyEventState keys, String target,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            double forwardInput = 0;
            double sideInput = 0;

            java.util.List keyList = viewAction.getKeyActions();
            foreach (Object k in keyList) {
                ViewInputAttributes.ActionAttributes.KeyAction keyAction =
                    (ViewInputAttributes.ActionAttributes.KeyAction) k;
                if (keys.isKeyDown(keyAction.keyCode))
                {
                    if (keyAction.direction == ViewInputAttributes.ActionAttributes.KeyAction.KA_DIR_X)
                    {
                        sideInput += keyAction.sign;
                    }
                    else
                    {
                        forwardInput += keyAction.sign;
                    }
                }

            }

            if (forwardInput == 0 && sideInput == 0)
            {
                return false;
            }


            //noinspection StringEquality
            if (target == GENERATE_EVENTS)
            {
                onHorizontalTranslateRel(forwardInput, sideInput, forwardInput, sideInput,
                    getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_KEYBOARD),viewAction);
            }

            return true;

        }


    }

    public class VerticalTransActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler, KeyEventState keys, String target,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            double transInput = 0;
            java.util.List keyList = viewAction.getKeyActions();
            foreach (Object k in keyList) {
                ViewInputAttributes.ActionAttributes.KeyAction keyAction =
                    (ViewInputAttributes.ActionAttributes.KeyAction) k;

                if (keys.isKeyDown(keyAction.keyCode))
                    transInput += keyAction.sign;
            }

            if (transInput == 0)
            {
                return false;
            }

            //noinspection StringEquality
            if (target == GENERATE_EVENTS)
            {
                
                ViewInputAttributes.DeviceAttributes deviceAttributes =
                    getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_KEYBOARD);
               
                onVerticalTranslate(transInput, transInput, deviceAttributes, viewAction);
            }

            return true;
        }


    }

    public class RotateMouseActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(KeyEventState keys, String target,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((keys.getMouseModifiersEx() & buttonAction.mouseButton) != 0)
                {
                    handleThisEvent = true;
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }

            Point point = constrainToSourceBounds(getMousePoint(), getWorldWindow());
            Point lastPoint = constrainToSourceBounds(getLastMousePoint(), getWorldWindow());
            Point mouseDownPoint = constrainToSourceBounds(getMouseDownPoint(), getWorldWindow());
            if (point == null || lastPoint == null)
            {
                return false;
            }

            Point movement = ViewUtil.subtract(point, lastPoint);
            int headingInput = movement.x;
            int pitchInput = movement.y;
            Point totalMovement = ViewUtil.subtract(point, mouseDownPoint);
            int totalHeadingInput = totalMovement.x;
            int totalPitchInput = totalMovement.y;

            ViewInputAttributes.DeviceAttributes deviceAttributes =
                getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE);

            onRotateView(headingInput, pitchInput, totalHeadingInput, totalPitchInput,
                deviceAttributes, viewAction);
            return true;
        }


        public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
            java.awt.event.MouseEvent mouseEvent, ViewInputAttributes.ActionAttributes viewAction)
        {
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((mouseEvent.getModifiersEx() & buttonAction.mouseButton) != 0)
                {
                    handleThisEvent = true;    
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }
            Point point = constrainToSourceBounds(getMousePoint(), getWorldWindow());
            Point lastPoint = constrainToSourceBounds(getLastMousePoint(), getWorldWindow());
            Point mouseDownPoint = constrainToSourceBounds(getMouseDownPoint(), getWorldWindow());
            if (point == null || lastPoint == null)
            {
                return false;
            }

            Point movement = ViewUtil.subtract(point, lastPoint);
            int headingInput = movement.x;
            int pitchInput = movement.y;
            if (mouseDownPoint == null)
                mouseDownPoint = lastPoint;
            Point totalMovement = ViewUtil.subtract(point, mouseDownPoint);
            int totalHeadingInput = totalMovement.x;
            int totalPitchInput = totalMovement.y;



            ViewInputAttributes.DeviceAttributes deviceAttributes =
                getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE);

            onRotateView(headingInput, pitchInput, totalHeadingInput, totalPitchInput,
                deviceAttributes, viewAction);
            return true;
        }
    }

    public class HorizTransMouseActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(
            KeyEventState keys, String target, ViewInputAttributes.ActionAttributes viewAction)
        {
            
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((keys.getMouseModifiersEx() & buttonAction.mouseButton) != 0)
                {
                    handleThisEvent = true;
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }
            if (target == GENERATE_EVENTS)
            {
                Point point = constrainToSourceBounds(getMousePoint(), getWorldWindow());
                Point lastPoint = constrainToSourceBounds(getLastMousePoint(), getWorldWindow());
                Point mouseDownPoint = constrainToSourceBounds(getMouseDownPoint(), getWorldWindow());

                Point movement = ViewUtil.subtract(point, lastPoint);
                if (point == null || lastPoint == null)
                    return false;
                int forwardInput = movement.y;
                int sideInput = -movement.x;

                Point totalMovement = ViewUtil.subtract(point, mouseDownPoint);
                int totalForward = totalMovement.y;
                int totalSide = -totalMovement.x;

                ViewInputAttributes.DeviceAttributes deviceAttributes =
                    getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE);

                onHorizontalTranslateRel(forwardInput, sideInput, totalForward, totalSide, deviceAttributes,
                    viewAction);
            }

            return(true);
        }

        public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
            java.awt.event.MouseEvent mouseEvent, ViewInputAttributes.ActionAttributes viewAction)
        {
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((mouseEvent.getModifiersEx() & buttonAction.mouseButton) != 0)
                {
                    handleThisEvent = true;
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }
            Point point = constrainToSourceBounds(getMousePoint(), getWorldWindow());
            Point lastPoint = constrainToSourceBounds(getLastMousePoint(), getWorldWindow());
            Point mouseDownPoint = constrainToSourceBounds(getMouseDownPoint(), getWorldWindow());
            if (point == null || lastPoint == null || mouseDownPoint == null)
            {
                return(false);
            }
            Point movement = ViewUtil.subtract(point, lastPoint);
            int forwardInput = movement.y;
            int sideInput = -movement.x;

            Point totalMovement = ViewUtil.subtract(point, mouseDownPoint);
            int totalForward = totalMovement.y;
            int totalSide = -totalMovement.x;

            ViewInputAttributes.DeviceAttributes deviceAttributes =
                getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE);

            onHorizontalTranslateRel(forwardInput, sideInput, totalForward, totalSide, deviceAttributes,
                viewAction);

            return(true);
        }
    }


    public class VertTransMouseActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
            java.awt.event.MouseEvent mouseEvent, ViewInputAttributes.ActionAttributes viewAction)
        {
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (java.lang.Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((mouseEvent.getModifiersEx() & buttonAction.mouseButton) != 0)
                {
                    handleThisEvent = true;
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }

            Point point = constrainToSourceBounds(getMousePoint(), getWorldWindow());
            Point lastPoint = constrainToSourceBounds(getLastMousePoint(), getWorldWindow());
            Point mouseDownPoint = constrainToSourceBounds(getMouseDownPoint(), getWorldWindow());
            if (point == null || lastPoint == null || mouseDownPoint == null)
            {
                return false;
            }

            Point movement = ViewUtil.subtract(point, lastPoint);
            int translationInput = movement.y;
            Point totalMovement = ViewUtil.subtract(point, mouseDownPoint);
            int totalTranslationInput = totalMovement.y;


            ViewInputAttributes.DeviceAttributes deviceAttributes =
                getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE);
            onVerticalTranslate((double) translationInput, totalTranslationInput, deviceAttributes, viewAction);

            return true;
        }

        public bool inputActionPerformed(
            KeyEventState keys, String target, ViewInputAttributes.ActionAttributes viewAction)
        {
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (java.lang.Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((keys.getMouseModifiersEx() & buttonAction.mouseButton) != 0)
                {
                    handleThisEvent = true;
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }

            Point point = constrainToSourceBounds(getMousePoint(), getWorldWindow());
            Point lastPoint = constrainToSourceBounds(getLastMousePoint(), getWorldWindow());
            Point mouseDownPoint = constrainToSourceBounds(getMouseDownPoint(), getWorldWindow());
            if (point == null || lastPoint == null)
            {
                return false;
            }

            Point movement = ViewUtil.subtract(point, lastPoint);
            int translationInput = movement.y;
            Point totalMovement = ViewUtil.subtract(point, mouseDownPoint);
            int totalTranslationInput = totalMovement.y;


            ViewInputAttributes.DeviceAttributes deviceAttributes =
                getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE);
            onVerticalTranslate((double) translationInput, totalTranslationInput, deviceAttributes, viewAction);

            return true;
        }
    }

    public class MoveToMouseActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
            java.awt.event.MouseEvent mouseEvent, ViewInputAttributes.ActionAttributes viewAction)
        {
            bool handleThisEvent = false;
            java.util.List buttonList = viewAction.getMouseActions();
            foreach (Object b in buttonList) {
                ViewInputAttributes.ActionAttributes.MouseAction buttonAction =
                    (ViewInputAttributes.ActionAttributes.MouseAction) b;
                if ((mouseEvent.getButton() ==  buttonAction.mouseButton))
                {
                    handleThisEvent = true;
                }
            }
            if (!handleThisEvent)
            {
                return false;
            }
            Position pos = computeSelectedPosition();
            if (pos == null)
            {
                return false;
            }

            onMoveTo(pos, getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE), viewAction);
            return true;
        }
    }

    public class ResetHeadingActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler, java.awt.event.KeyEvent event,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            java.util.List keyList = viewAction.getKeyActions();
            foreach (Object k in keyList)
            {
                ViewInputAttributes.ActionAttributes.KeyAction keyAction =
                    (ViewInputAttributes.ActionAttributes.KeyAction) k;
                if (event.getKeyCode() == keyAction.keyCode)
                {
                    onResetHeading(viewAction);
                    return true;
                }
            }
            return false;
        }

    }

    public class ResetHeadingPitchActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler, java.awt.event.KeyEvent event,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            java.util.List keyList = viewAction.getKeyActions();
            foreach (Object k in keyList)
            {
                ViewInputAttributes.ActionAttributes.KeyAction keyAction =
                    (ViewInputAttributes.ActionAttributes.KeyAction) k;
                if (event.getKeyCode() == keyAction.keyCode)
                {
                    onResetHeadingPitchRoll(viewAction);
                    return true;
                }
            }
            return false;
        }
    }

    public class StopViewActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler, java.awt.event.KeyEvent event,
            ViewInputAttributes.ActionAttributes viewAction)
        {
            java.util.List keyList = viewAction.getKeyActions();
            foreach (Object k in keyList)
            {
                ViewInputAttributes.ActionAttributes.KeyAction keyAction =
                    (ViewInputAttributes.ActionAttributes.KeyAction) k;
                if (event.getKeyCode() == keyAction.keyCode)
                {
                    onStopView();
                    return true;
                }
            }
            return false;
        }
       
    }

    public class VertTransMouseWheelActionListener : ViewInputActionHandler
    {
        public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
            java.awt.event.MouseWheelEvent mouseWheelEvent, ViewInputAttributes.ActionAttributes viewAction)
        {

            double zoomInput = mouseWheelEvent.getWheelRotation();

            ViewInputAttributes.DeviceAttributes deviceAttributes =
                getAttributes().getDeviceAttributes(ViewInputAttributes.DEVICE_MOUSE_WHEEL);

            onVerticalTranslate(zoomInput, zoomInput, deviceAttributes, viewAction);
            return true;

        }
    }



    public BasicViewInputHandler()
    {

        ViewInputAttributes.ActionAttributes actionAttrs;
        // Get action maps for mouse and keyboard
        // ----------------------------------Key Rotation --------------------------------------------
        RotateActionListener rotateActionListener = new RotateActionListener();
        this.getAttributes().setActionListener(
            ViewInputAttributes.DEVICE_KEYBOARD, ViewInputAttributes.VIEW_ROTATE_KEYS, rotateActionListener);
        this.getAttributes().setActionListener(
            ViewInputAttributes.DEVICE_KEYBOARD, ViewInputAttributes.VIEW_ROTATE_SLOW, rotateActionListener);
        this.getAttributes().setActionListener(
            ViewInputAttributes.DEVICE_KEYBOARD, ViewInputAttributes.VIEW_ROTATE_KEYS_SHIFT, rotateActionListener);
        this.getAttributes().setActionListener(
            ViewInputAttributes.DEVICE_KEYBOARD, ViewInputAttributes.VIEW_ROTATE_KEYS_SHIFT_SLOW, rotateActionListener);

        // ----------------------------------Key Vertical Translation --------------------------------
        VerticalTransActionListener vertActionsListener = new VerticalTransActionListener();
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_VERTICAL_TRANS_KEYS_CTRL);
        actionAttrs.setActionListener(vertActionsListener);
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_VERTICAL_TRANS_KEYS_SLOW_CTRL);
        actionAttrs.setActionListener(vertActionsListener);
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_VERTICAL_TRANS_KEYS);
        actionAttrs.setActionListener(vertActionsListener);
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_VERTICAL_TRANS_KEYS_SLOW);
        actionAttrs.setActionListener(vertActionsListener);

        // ----------------------------------Key Horizontal Translation ------------------------------
        HorizontalTransActionListener horizTransActionListener = new HorizontalTransActionListener();
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_HORIZONTAL_TRANS_KEYS);
        actionAttrs.setActionListener(horizTransActionListener);
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_HORIZONTAL_TRANSLATE_SLOW);
        actionAttrs.setActionListener(horizTransActionListener);

        // -------------------------------- Mouse Rotation -------------------------------------------
        RotateMouseActionListener rotateMouseListener = new RotateMouseActionListener();
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE).getActionAttributes(
                ViewInputAttributes.VIEW_ROTATE);
        actionAttrs.setMouseActionListener(rotateMouseListener);
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE).getActionAttributes(
                ViewInputAttributes.VIEW_ROTATE_SHIFT);
        actionAttrs.setMouseActionListener(rotateMouseListener);

        // ----------------------------- Mouse Horizontal Translation --------------------------------
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE).getActionAttributes(
                ViewInputAttributes.VIEW_HORIZONTAL_TRANSLATE);
        actionAttrs.setMouseActionListener(new HorizTransMouseActionListener());

        // ----------------------------- Mouse Vertical Translation ----------------------------------
        VertTransMouseActionListener vertTransListener = new VertTransMouseActionListener();
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE).getActionAttributes(
                ViewInputAttributes.VIEW_VERTICAL_TRANSLATE);
        actionAttrs.setMouseActionListener(vertTransListener);
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE).getActionAttributes(
                ViewInputAttributes.VIEW_VERTICAL_TRANSLATE_CTRL);
        actionAttrs.setMouseActionListener(vertTransListener);

        // Move to mouse
        actionAttrs = this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE).getActionAttributes(
                ViewInputAttributes.VIEW_MOVE_TO);
        actionAttrs.setMouseActionListener(new MoveToMouseActionListener());

        // Reset Heading
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_RESET_HEADING);
        actionAttrs.setActionListener(new ResetHeadingActionListener());

        // Reset Heading and Pitch
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_RESET_HEADING_PITCH_ROLL);
        actionAttrs.setActionListener(new ResetHeadingPitchActionListener());

        // Stop View
        actionAttrs =
            this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_KEYBOARD).getActionAttributes(
                ViewInputAttributes.VIEW_STOP_VIEW);
        actionAttrs.setActionListener(new StopViewActionListener());


        // Mouse Wheel vertical translate
        actionAttrs = this.getAttributes().getActionMap(ViewInputAttributes.DEVICE_MOUSE_WHEEL).getActionAttributes(
            ViewInputAttributes.VIEW_VERTICAL_TRANSLATE);
        actionAttrs.setMouseActionListener(new VertTransMouseWheelActionListener());

        

        
    }

    public void apply()
    {
        base.apply();
    }

    //**************************************************************//
    //********************  Key Events  ****************************//
    //**************************************************************//
    protected void handleKeyPressed(KeyEvent e)
    {

        bool eventHandled = false;


        Integer modifier =  e.getModifiersEx();
        for (int i = 0; i < NUM_MODIFIERS; i++)
        {
            if ((((modifier & this.modifierList[i]) == this.modifierList[i])))
            {
                ViewInputAttributes.ActionAttributesList actionList = (ViewInputAttributes.ActionAttributesList)
                    keyModsActionMap.get(this.modifierList[i]);
                eventHandled = callActionListListeners(e,
                    ViewInputAttributes.ActionAttributes.ActionTrigger.ON_PRESS, actionList);
            }
        }

        if (!eventHandled)
        {
            base.handleKeyPressed(e);    
        }


    }

    //**************************************************************//
    //********************  Mouse Events  **************************//
    //**************************************************************//
    protected void handleMouseClicked(MouseEvent e)
    {

        bool eventHandled = false;


        Integer modifier =  e.getModifiersEx();
        for (int i = 0; i < NUM_MODIFIERS; i++)
        {
            if ((((modifier & this.modifierList[i]) == this.modifierList[i])))
            {
                ViewInputAttributes.ActionAttributesList actionList = (ViewInputAttributes.ActionAttributesList)
                    mouseModsActionMap.get(this.modifierList[i]);
                eventHandled = callMouseActionListListeners(e,
                    ViewInputAttributes.ActionAttributes.ActionTrigger.ON_PRESS, actionList);
            }
        }

        if (!eventHandled)
        {
            base.handleMouseClicked(e);
        }

    }

    protected void handleMouseWheelMoved(MouseWheelEvent e)
    {
        bool eventHandled = false;

        // TODO : Make this conditional look like handleMouseDragged
        Integer modifier =  e.getModifiersEx();
        for (int i = 0; i < NUM_MODIFIERS; i++)
        {
            if ((((modifier & this.modifierList[i]) == this.modifierList[i])))
            {
                ViewInputAttributes.ActionAttributesList actionList = (ViewInputAttributes.ActionAttributesList)
                    mouseWheelModsActionMap.get(this.modifierList[i]);
                eventHandled = callMouseWheelActionListListeners(e,
                    ViewInputAttributes.ActionAttributes.ActionTrigger.ON_DRAG, actionList);
            }
        }

        if (!eventHandled)
        {
            base.handleMouseWheelMoved(e);
        }
    }



    //**************************************************************//
    //********************  Mouse Motion Events  *******************//
    //**************************************************************//
    protected void handleMouseDragged(MouseEvent e)
    {

        int modifier =  e.getModifiersEx();

        for (int i = 0; i < NUM_MODIFIERS; i++)
        {
            if ((((modifier & this.modifierList[i]) == this.modifierList[i])))
            {
                ViewInputAttributes.ActionAttributesList actionList = (ViewInputAttributes.ActionAttributesList)
                    mouseModsActionMap.get(this.modifierList[i]);
                if (callMouseActionListListeners(e,
                    ViewInputAttributes.ActionAttributes.ActionTrigger.ON_DRAG, actionList))
                {
                    return;
                }
            }
        }
        
    }



    //**************************************************************//
    //********************  Rendering Events  **********************//
    //**************************************************************//

    // Interpret the current key state according to the specified target. If the target is KEY_POLL_GENERATE_EVENTS,
    // then the the key state will generate any appropriate view change events. If the target is KEY_POLL_QUERY_EVENTS,
    // then the key state will not generate events, and this will return whether or not any view change events would
    // have been generated.
    protected bool handlePerFrameKeyState(KeyEventState keys, String target)
    {

        if (keys.getNumKeysDown() == 0)
        {
            return false;
        }
        bool isKeyEventTrigger = false;


        Integer modifier =  keys.getModifiersEx();
        for (int i = 0; i < NUM_MODIFIERS; i++)
        {
            if (((modifier & this.modifierList[i]) == this.modifierList[i]))
            {

                ViewInputAttributes.ActionAttributesList actionList = (ViewInputAttributes.ActionAttributesList)
                    keyModsActionMap.get(this.modifierList[i]);
                isKeyEventTrigger = callActionListListeners(keys, target,
                    ViewInputAttributes.ActionAttributes.ActionTrigger.ON_KEY_DOWN, actionList);
                break;
            }
        }

        return isKeyEventTrigger;
    }

    protected bool handlePerFrameMouseState(KeyEventState keys, String target)
    {
        bool eventHandled = false;

        if (keys.getNumButtonsDown() == 0)
        {
            return false;
        }


        Integer modifier =  keys.getModifiersEx();

        for (int i = 0; i < NUM_MODIFIERS; i++)
        {
            if (((modifier & this.modifierList[i]) == this.modifierList[i]))
            {

                ViewInputAttributes.ActionAttributesList actionList = (ViewInputAttributes.ActionAttributesList)
                    mouseModsActionMap.get(this.modifierList[i]);
                if (callActionListListeners(keys, target,
                    ViewInputAttributes.ActionAttributes.ActionTrigger.ON_KEY_DOWN, actionList))
                {
                    
                    return true;
                }
            }
        }
        
        return(eventHandled);

    }

    protected bool callMouseActionListListeners(MouseEvent e,
        ViewInputAttributes.ActionAttributes.ActionTrigger trigger,
        ViewInputAttributes.ActionAttributesList actionList)
    {
        bool eventHandled = false;
        if (actionList != null)
        {
            foreach (ViewInputAttributes.ActionAttributes actionAttr in actionList)
            {
                if (actionAttr.getMouseActionListener() == null ||
                    actionAttr.getActionTrigger() != trigger)
                {
                    continue;
                }
                if (actionAttr.getMouseActionListener().inputActionPerformed(this, e, actionAttr))
                {
                    eventHandled = true;
                }
            }

        }
        return eventHandled;
    }

    protected bool callMouseWheelActionListListeners(MouseWheelEvent e,
        ViewInputAttributes.ActionAttributes.ActionTrigger trigger,
        ViewInputAttributes.ActionAttributesList actionList)
    {
        bool eventHandled = false;
        if (actionList != null)
        {
            foreach (ViewInputAttributes.ActionAttributes actionAttr in actionList)
            {
                if (actionAttr.getMouseActionListener() == null ||
                    actionAttr.getActionTrigger() != trigger)
                {
                    continue;
                }
                if (actionAttr.getMouseActionListener().inputActionPerformed(this, e, actionAttr))
                {
                    eventHandled = true;
                }
            }

        }
        return eventHandled;
    }

    protected bool callActionListListeners(KeyEvent e,
        ViewInputAttributes.ActionAttributes.ActionTrigger trigger,
        ViewInputAttributes.ActionAttributesList actionList)
    {
        bool eventHandled = false;
        if (actionList != null)
        {
            foreach (ViewInputAttributes.ActionAttributes actionAttr in actionList)
            {
                if (actionAttr.getActionListener() == null ||
                    actionAttr.getActionTrigger() != trigger)
                {
                    continue;
                }
                if (actionAttr.getActionListener().inputActionPerformed(this, e, actionAttr))
                {
                    eventHandled = true;
                }
            }
        }
        return eventHandled;
    }

    protected bool callActionListListeners(KeyEventState keys, String target,
        ViewInputAttributes.ActionAttributes.ActionTrigger trigger,
        ViewInputAttributes.ActionAttributesList actionList)
    {
        bool eventHandled = false;
        if (actionList != null)
        {
            foreach (ViewInputAttributes.ActionAttributes actionAttr in actionList)
            {
                if (actionAttr.getActionTrigger() != trigger)
                {
                    continue;
                }

                if (callActionListener(keys, target, actionAttr))
                {
                    eventHandled = true;
                }

            }

        }
        return eventHandled;
    }

    protected bool callActionListener (KeyEventState keys, String target,
        ViewInputAttributes.ActionAttributes action)
    {

        if (action.getActionListener() != null)
        {
            return(action.getActionListener().inputActionPerformed(this, keys, target, action));
        }
        if (action.getMouseActionListener() != null)
        {
            return(action.getMouseActionListener().inputActionPerformed(keys, target, action));
        }
        return false;

    }

    //**************************************************************//
    //********************  Property Change Events  ****************//
    //**************************************************************//

    protected void handlePropertyChange(java.beans.PropertyChangeEvent e)
    {
        base.handlePropertyChange(e);

            //noinspection StringEquality
        if (e.getPropertyName() == ViewContansts.VIEW_STOPPED)
        {
            this.handleViewStopped();
        }
    }

    protected void handleViewStopped()
    {

    }
}
}
