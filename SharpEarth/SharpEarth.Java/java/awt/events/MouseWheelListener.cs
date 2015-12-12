using java.util;

namespace java.awt.events
{
  public interface MouseWheelListener : EventListener
  {
    /**
 * Invoked when the mouse wheel is rotated.
 * @see MouseWheelEvent
 */
    void mouseWheelMoved( MouseWheelEvent e );
  }
}
