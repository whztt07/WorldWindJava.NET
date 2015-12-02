/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.awt{

/**
 * @author jym
 * @version $Id: KeyInputActionHandler.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface KeyInputActionHandler
{
    public bool inputActionPerformed(AbstractViewInputHandler inputHandler, KeyEventState keys, String target,
        ViewInputAttributes.ActionAttributes viewAction);
    public bool inputActionPerformed(AbstractViewInputHandler inputHandler, java.awt.event.KeyEvent event,
        ViewInputAttributes.ActionAttributes viewAction);

}
}
