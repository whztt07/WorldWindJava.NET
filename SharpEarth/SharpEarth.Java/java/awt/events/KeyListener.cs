﻿using java.util;

namespace java.awt.events
{
  /**
   * The listener interface for receiving keyboard events (keystrokes).
   * The class that is interested in processing a keyboard event
   * either implements this interface (and all the methods it
   * contains) or extends the abstract <code>KeyAdapter</code> class
   * (overriding only the methods of interest).
   * <P>
   * The listener object created from that class is then registered with a
   * component using the component's <code>addKeyListener</code>
   * method. A keyboard event is generated when a key is pressed, released,
   * or typed. The relevant method in the listener
   * object is then invoked, and the <code>KeyEvent</code> is passed to it.
   *
   * @author Carl Quinn
   *
   * @see KeyAdapter
   * @see KeyEvent
   * @see <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/keylistener.html">Tutorial: Writing a Key Listener</a>
   *
   * @since 1.1
   */
  public interface KeyListener : EventListener
  {

    /**
     * Invoked when a key has been typed.
     * See the class description for {@link KeyEvent} for a definition of
     * a key typed event.
     */
    void keyTyped(KeyEvent e);

    /**
     * Invoked when a key has been pressed.
     * See the class description for {@link KeyEvent} for a definition of
     * a key pressed event.
     */
    void keyPressed(KeyEvent e);

    /**
     * Invoked when a key has been released.
     * See the class description for {@link KeyEvent} for a definition of
     * a key released event.
     */
    void keyReleased(KeyEvent e);
  }
}
