/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.util{

/**
 * @author tag
 * @version $Id: TaskService.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface TaskService
{
    void shutdown(boolean immediately);

    bool contains(Runnable runnable);

    /**
     * Enqueues a task to run.
     *
     * @param runnable the task to add
     * @throws ArgumentException if <code>runnable</code> is null
     */
    void addTask(Runnable runnable);

    bool isFull();

    bool hasActiveTasks();
}
}
