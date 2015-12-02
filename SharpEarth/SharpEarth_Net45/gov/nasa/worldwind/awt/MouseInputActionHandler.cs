/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.awt{

/**
 * @author jym
 * @version $Id: MouseInputActionHandler.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface MouseInputActionHandler
{
    public bool inputActionPerformed(KeyEventState keys, String target,
        ViewInputAttributes.ActionAttributes viewAction);

    public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
        java.awt.event.MouseEvent mouseEvent, ViewInputAttributes.ActionAttributes viewAction);

    public bool inputActionPerformed(AbstractViewInputHandler inputHandler,
            java.awt.event.MouseWheelEvent mouseWheelEvent, ViewInputAttributes.ActionAttributes viewAction);
}
}
