/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using System.Linq;
using java.util;
namespace SharpEarth.animation{


/**
 * The <code>AnimationController</code> class is a convenience class for managing a
 * group of <code>Animators</code>.
 *
 * @author jym
 * @version $Id: AnimationController.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class AnimationController :  Dictionary<string, Animator>
{

    /**
     * Starts all of the <code>Animator</code>s in the map
     */
    public void startAnimations()
    {
      foreach ( var animator in Values )
      {
        animator.start();
      }
    }

    /**
     * Stops all of the <code>Animator</code>s in the map
     */
    public void stopAnimations()
    {
      foreach ( var animator in Values )
      {
        animator.stop();
      }
    }

    /**
     * Starts the animation associated with <code>animationName</code>
     *
     * @param animationName the name of the animation to be started.
     */
    public bool startAnimation(string animationName)
    {
      Animator animator;
      if ( TryGetValue( animationName, out animator ) )
      {
        animator.start();
        return true;
      }
      return false;
    }

    /**
     * Stops the <code>Animator</code> associated with <code>animationName</code>
     * @param animationName the name of the animation to be stopped
     */
    public bool stopAnimation( string animationName )
    {
      Animator animator;
      if ( TryGetValue( animationName, out animator ) )
      {
        animator.stop();
        return true;
      }
      return false;
    }

    /**
     * Stops all <code>Animator</code>s in the map.
     * @return true if any <code>Animator</code> was started, false otherwise
     */
    public bool stepAnimators()
    {
      var steps = Values.Where( animator => animator.hasNext() ).ToArray();
      foreach ( var step in steps )
      {
        step.next();
      }
      return steps.Length > 0;
    }

    /**
     * Returns <code>true</code> if the controller has any active <code>Animations</code>
     * 
     * @return true if there are any active animations in this <code>CompountAnimation</code>
     */
    public bool hasActiveAnimation()
    {
      return Values.Any( animator => animator.hasNext() );
    }
}
}
