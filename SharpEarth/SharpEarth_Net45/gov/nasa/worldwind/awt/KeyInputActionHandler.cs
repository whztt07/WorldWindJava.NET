/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.awt.events;

namespace SharpEarth.awt{

/**
 * @author jym
 * @version $Id: KeyInputActionHandler.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface KeyInputActionHandler
{
    bool inputActionPerformed(AbstractViewInputHandler inputHandler, KeyEventState keys, string target,

        ViewInputAttributes.ActionAttributes viewAction);
    bool inputActionPerformed(AbstractViewInputHandler inputHandler, KeyEvent keyEvent,
        ViewInputAttributes.ActionAttributes viewAction);

}
}
