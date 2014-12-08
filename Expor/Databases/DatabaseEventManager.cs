﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases
{

    public class DatabaseEventManager
    {
        /**
         * Holds the listener.
         */
        //private EventListenerList listenerList = new EventListenerList();

        ///**
        // * Indicates whether DataStoreEvents should be accumulated and fired as one event on
        // * demand.
        // */
        //private bool accumulateDataStoreEvents = false;

        ///**
        // * The type of the current DataStoreEvent to be accumulated.
        // */
        //private DataStoreEvent.Type currentDataStoreEventType = null;

        ///**
        // * The objects that were changed in the current DataStoreEvent.
        // */
        //private IHashSetModifiableDbIds dataStoreObjects;

        ///**
        // * Collects successive insertion, deletion or update events. The accumulated
        // * event will be fired when {@link #flushDataStoreEvents()} is called or a
        // * different event type occurs.
        // * 
        // * @see #flushDataStoreEvents()
        // * @see DataStoreEvent
        // */
        //public void accumulateDataStoreEvents() {
        //  this.accumulateDataStoreEvents = true;
        //}

        ///**
        // * Fires all collected insertion, deletion or update events as one
        // * DataStoreEvent, i.e. notifies all registered DataStoreListener how the
        // * content of the database has been changed since
        // * {@link #accumulateDataStoreEvents()} was called.
        // * 
        // * @see #accumulateDataStoreEvents
        // * @see DataStoreListener
        // * @see DataStoreEvent
        // */
        //public void flushDataStoreEvents() {
        //  // inform listeners
        //  Object[] listeners = listenerList.getListenerList();
        //  Map<Type, DbIds> objects = new HashMap<Type, DbIds>();
        //  objects.put(currentDataStoreEventType, DbIdUtil.makeUnmodifiable(dataStoreObjects));
        //  DataStoreEvent e = new DataStoreEvent(this, objects);

        //  for(int i = listeners.length - 2; i >= 0; i -= 2) {
        //    if(listeners[i] == DataStoreListener.class) {
        //      ((DataStoreListener) listeners[i + 1]).contentChanged(e);
        //    }
        //  }
        //  // reset
        //  accumulateDataStoreEvents = false;
        //  currentDataStoreEventType = null;
        //  dataStoreObjects = null;
        //}

        ///**
        // * Adds a <code>DataStoreListener</code> for a <code>DataStoreEvent</code>
        // * posted after the content of the database changes.
        // * 
        // * @param l the listener to add
        // * @see #removeListener(DataStoreListener)
        // * @see DataStoreListener
        // * @see DataStoreEvent
        // */
        //public void addListener(DataStoreListener l) {
        //  listenerList.add(DataStoreListener.class, l);
        //}

        ///**
        // * Removes a <code>DataStoreListener</code> previously added with
        // * {@link #addListener(DataStoreListener)}.
        // * 
        // * @param l the listener to remove
        // * @see #addListener(DataStoreListener)
        // * @see DataStoreListener
        // * @see DataStoreEvent
        // */
        //public void removeListener(DataStoreListener l) {
        //  listenerList.remove(DataStoreListener.class, l);
        //}

        ///**
        // * Adds a <code>ResultListener</code> to be notified on new results.
        // * 
        // * @param l the listener to add
        // * @see #removeListener(ResultListener)
        // * @see ResultListener
        // * @see Result
        // */
        //public void addListener(ResultListener l) {
        //  listenerList.add(ResultListener.class, l);
        //}

        ///**
        // * Removes a <code>ResultListener</code> previously added with
        // * {@link #addListener(ResultListener)}.
        // * 
        // * @param l the listener to remove
        // * @see #addListener(ResultListener)
        // * @see ResultListener
        // * @see Result
        // * 
        // */
        //public void removeListener(ResultListener l) {
        //  listenerList.remove(ResultListener.class, l);
        //}

        ///**
        // * Convenience method, calls {@code fireObjectsChanged(insertions,
        // * DataStoreEvent.Type.INSERT)}.
        // * 
        // * @param insertions the objects that have been inserted
        // * @see #fireObjectsChanged
        // * @see de.lmu.ifi.dbs.elki.database.datastore.DataStoreEvent.Type#INSERT
        // */
        //public void fireObjectsInserted(DbIds insertions) {
        //  fireObjectsChanged(insertions, DataStoreEvent.Type.INSERT);
        //}

        ///**
        // * Convenience method, calls {@code fireObjectChanged(insertion,
        // * DataStoreEvent.Type.INSERT)}.
        // * 
        // * @param insertion the object that has been inserted
        // * @see #fireObjectsChanged
        // * @see de.lmu.ifi.dbs.elki.database.datastore.DataStoreEvent.Type#INSERT
        // */
        //public void fireObjectInserted(DbId insertion) {
        //  fireObjectsChanged(insertion, DataStoreEvent.Type.INSERT);
        //}

        ///**
        // * Convenience method, calls {@code fireObjectsChanged(updates,
        // * DataStoreEvent.Type.UPDATE)}.
        // * 
        // * @param updates the objects that have been updated
        // * @see #fireObjectsChanged
        // * @see de.lmu.ifi.dbs.elki.database.datastore.DataStoreEvent.Type#UPDATE
        // */
        //public void fireObjectsUpdated(DbIds updates) {
        //  fireObjectsChanged(updates, DataStoreEvent.Type.UPDATE);
        //}

        ///**
        // * Convenience method, calls {@code fireObjectsChanged(deletions,
        // * DataStoreEvent.Type.DELETE)}.
        // * 
        // * @param deletions the objects that have been removed
        // * @see #fireObjectsChanged
        // * @see de.lmu.ifi.dbs.elki.database.datastore.DataStoreEvent.Type#DELETE
        // */
        //protected void fireObjectsRemoved(DbIds deletions) {
        //  fireObjectsChanged(deletions, DataStoreEvent.Type.DELETE);
        //}

        ///**
        // * Convenience method, calls {@code fireObjectChanged(deletion,
        // * DataStoreEvent.Type.DELETE)}.
        // * 
        // * @param deletion the object that has been removed
        // * @see #fireObjectsChanged
        // * @see de.lmu.ifi.dbs.elki.database.datastore.DataStoreEvent.Type#DELETE
        // */
        //protected void fireObjectRemoved(DbId deletion) {
        //  fireObjectsChanged(deletion, DataStoreEvent.Type.DELETE);
        //}

        ///**
        // * Handles a DataStoreEvent with the specified type. If the current event type
        // * is not equal to the specified type, the events accumulated up to now will
        // * be fired first.
        // * 
        // * The new event will be aggregated and fired on demand if
        // * {@link #accumulateDataStoreEvents} is set, otherwise all registered
        // * <code>DataStoreListener</code> will be notified immediately that the
        // * content of the database has been changed.
        // * 
        // * @param objects the objects that have been changed, i.e. inserted, deleted
        // *        or updated
        // */
        //private void fireObjectsChanged(DbIds objects, DataStoreEvent.Type type) {
        //  // flush first
        //  if(currentDataStoreEventType != null && !currentDataStoreEventType.equals(type)) {
        //    flushDataStoreEvents();
        //  }
        //  if (this.dataStoreObjects == null) {
        //    this.dataStoreObjects = DbIdUtil.newHashSet();
        //  }
        //  this.dataStoreObjects.addDbIds(objects);
        //  currentDataStoreEventType = type;

        //  if(!accumulateDataStoreEvents) {
        //    flushDataStoreEvents();
        //  }
        //}

        ///**
        // * Informs all registered <code>ResultListener</code> that a new result was
        // * added.
        // * 
        // * @param r New child result added
        // * @param parent Parent result that was added to
        // */
        //public void fireResultAdded(Result r, Result parent) {
        //  for(ResultListener l : listenerList.getListeners(ResultListener.class)) {
        //    l.resultAdded(r, parent);
        //  }
        //}

        ///**
        // * Informs all registered <code>ResultListener</code> that a new result has
        // * been removed.
        // * 
        // * @param r result that has been removed
        // * @param parent Parent result that has been removed
        // */
        //public void fireResultRemoved(Result r, Result parent) {
        //  for(ResultListener l : listenerList.getListeners(ResultListener.class)) {
        //    l.resultRemoved(r, parent);
        //  }
        //}

    }

}
