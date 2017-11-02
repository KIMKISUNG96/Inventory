﻿using NUnit.Framework;
using FarrokhGames.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    [TestFixture]
    public class InventoryManagerTests
    {
        IInventoryItem CreateFullItem(int width, int height)
        {
            var shape = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    shape[x, y] = true;
                }
            }
            return new TestItem("item", null, new InventoryShape(shape));
        }

        /*
        Constructor
        */

        [Test]
        public void CTOR_WidthAndHeightSet()
        {
            var inventory = new InventoryManager(16, 8);
            Assert.That(inventory.Width, Is.EqualTo(16));
            Assert.That(inventory.Height, Is.EqualTo(8));
        }

        /*
        AllItems
        */

        [Test]
        public void AllItems_ReturnsListOfAllItems()
        {
            var inventory = new InventoryManager(16, 8);

            var item1 = CreateFullItem(1, 1);
            inventory.Add(item1);

            var item2 = CreateFullItem(2, 2);
            inventory.Add(item2);

            var allItems = inventory.AllItems;
            Assert.That(allItems.Count, Is.EqualTo(2));
            Assert.That(allItems.Contains(item1), Is.True);
            Assert.That(allItems.Contains(item2), Is.True);
        }

        [Test]
        public void AllItems_ReturnsCopy()
        {
            var inventory = new InventoryManager(16, 8);
            inventory.Add(CreateFullItem(1, 1));
            Assert.That(inventory.AllItems, Is.Not.SameAs(inventory.AllItems));
        }

        /*
        Contains
        */

        [TestCase(false, ExpectedResult = false)]
        [TestCase(true, ExpectedResult = true)]
        public bool Contains(bool doAdd)
        {
            var inventory = new InventoryManager(16, 8);
            var item1 = CreateFullItem(1, 1);
            if (doAdd) { inventory.Add(item1); }
            return inventory.Contains(item1);
        }

        /*
        IsFull
        */

        [Test]
        public void IsFull_Empty_ReturnsFalse()
        {
            var inventory = new InventoryManager(16, 8);
            Assert.That(inventory.IsFull, Is.False);
        }

        [Test]
        public void IsFull_NotFull_ReturnsFalse()
        {
            var inventory = new InventoryManager(4, 4);
            inventory.Add(CreateFullItem(2, 2));
            Assert.That(inventory.IsFull, Is.False);
        }

        [Test]
        public void IsFull_Full_ReturnsTrue()
        {
            var inventory = new InventoryManager(2, 2);
            inventory.Add(CreateFullItem(2, 2));
            Assert.That(inventory.IsFull, Is.True);
        }

        /*
        CanAdd
        */

        [Test]
        public void CanAdd_DoesFit_ReturnsTrue()
        {
            var inventory = new InventoryManager(8, 4);
            Assert.That(inventory.CanAdd(CreateFullItem(1, 1)), Is.True);
            Assert.That(inventory.CanAdd(CreateFullItem(8, 4)), Is.True);
            Assert.That(inventory.CanAdd(CreateFullItem(3, 3)), Is.True);
        }

        [Test]
        public void CanAdd_DoesNotFit_ReturnsFalse()
        {
            var inventory = new InventoryManager(8, 4);
            Assert.That(inventory.CanAdd(CreateFullItem(1, 5)), Is.False);
            Assert.That(inventory.CanAdd(CreateFullItem(9, 1)), Is.False);
            Assert.That(inventory.CanAdd(CreateFullItem(9, 5)), Is.False);
        }

        [Test]
        public void CanAdd_ItemAlreadyAdded_ReturnsFalse()
        {
            var inventory = new InventoryManager(8, 4);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            Assert.That(inventory.CanAdd(item), Is.False);
        }

        /*
        Add
        */

        [Test]
        public void Add_ItemDoesNotFit_NoItemAdded()
        {
            var inventory = new InventoryManager(2, 2);
            var item = CreateFullItem(3, 3);
            inventory.Add(item);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void Add_ItemDoesFit_ItemAdded()
        {
            var inventory = new InventoryManager(3, 3);
            var item = CreateFullItem(3, 3);
            inventory.Add(item);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_ItemAlreadyAdded_NoItemAdded()
        {
            var inventory = new InventoryManager(3, 3);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            inventory.Add(item);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_ItemAdded_OnItemAddedInvoked()
        {
            var inventory = new InventoryManager(3, 3);
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.OnItemAdded += (i) =>
            {
                callbacks++;
                lastItem = i;
            };

            var item = CreateFullItem(1, 1);
            inventory.Add(item);

            Assert.That(lastItem, Is.Not.Null);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void Add_ItemNotAdded_OnItemAddedNotInvoked()
        {
            var inventory = new InventoryManager(1, 1);
            var callbacks = 0;
            inventory.OnItemAdded += (i) => { callbacks++; };
            inventory.Add(CreateFullItem(2, 2));
            Assert.That(callbacks, Is.EqualTo(0));
        }

        /*
        CanAddAt
        */

        [Test]
        public void CanAddAt_DoesFit_ReturnsTrue()
        {
            var inventory = new InventoryManager(8, 4);
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 1), Point.one), Is.True);
            Assert.That(inventory.CanAddAt(CreateFullItem(8, 4), Point.zero), Is.True);
            Assert.That(inventory.CanAddAt(CreateFullItem(3, 3), Point.right), Is.True);
        }

        [Test]
        public void CanAddAt_DoesNotFit_ReturnsFalse()
        {
            var inventory = new InventoryManager(8, 4);
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 5), Point.zero), Is.False);
            Assert.That(inventory.CanAddAt(CreateFullItem(9, 1), Point.one), Is.False);
        }

        [Test]
        public void CanAddAt_ItemInTheWay_ReturnsFalse()
        {
            var inventory = new InventoryManager(3, 3);
            var item1 = CreateFullItem(3, 1);
            inventory.AddAt(item1, Point.zero);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 1), new Point(1, 0)), Is.False);
        }

        [Test]
        public void CanAddAt_OutsideInventory_ReturnsFalse()
        {
            var inventory = new InventoryManager(8, 4);
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 5), new Point(-1, 1)), Is.False);
            Assert.That(inventory.CanAddAt(CreateFullItem(9, 1), new Point(1, -1)), Is.False);
        }

        [Test]
        public void CanAddAt_ItemAlreadyAdded_ReturnsFalse()
        {
            var inventory = new InventoryManager(8, 4);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            Assert.That(inventory.CanAddAt(item, Point.zero), Is.False);
        }

        /*
        AddAt
        */

        [Test]
        public void AddAt_ItemDoesNotFit_NoItemAdded()
        {
            var inventory = new InventoryManager(2, 2);
            var item = CreateFullItem(3, 3);
            inventory.AddAt(item, Point.zero);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddAt_ItemDoesFit_ItemAdded()
        {
            var inventory = new InventoryManager(3, 3);
            var item = CreateFullItem(3, 3);
            inventory.AddAt(item, Point.zero);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemInTheWay_ItemNotAdded()
        {
            var inventory = new InventoryManager(3, 3);
            var item1 = CreateFullItem(3, 1);
            inventory.AddAt(item1, Point.zero);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
            var item2 = CreateFullItem(1, 1);
            inventory.AddAt(item2, new Point(1, 0));
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_OutsideInventory_ItemNotAdded()
        {
            var inventory = new InventoryManager(3, 3);
            var item1 = CreateFullItem(3, 1);
            inventory.AddAt(item1, new Point(-1, -1));
            Assert.That(inventory.AllItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddAt_ItemAlreadyAdded_NoItemAdded()
        {
            var inventory = new InventoryManager(3, 3);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            inventory.AddAt(item, Point.zero);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemAdded_ItemPositionUpdated()
        {
            var inventory = new InventoryManager(3, 3);
            var item = CreateFullItem(1, 1);
            inventory.AddAt(item, Point.one);
            Assert.That(item.Shape.Position, Is.EqualTo(Point.one));
        }

        [Test]
        public void AddAt_ItemAdded_OnItemAddedInvoked()
        {
            var inventory = new InventoryManager(3, 3);
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.OnItemAdded += (i) =>
            {
                callbacks++;
                lastItem = i;
            };

            var item = CreateFullItem(1, 1);
            inventory.AddAt(item, Point.zero);

            Assert.That(lastItem, Is.Not.Null);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemNotAdded_OnItemAddedNotInvoked()
        {
            var inventory = new InventoryManager(1, 1);
            var callbacks = 0;
            inventory.OnItemAdded += (i) => { callbacks++; };
            inventory.AddAt(CreateFullItem(2, 2), Point.zero);
            Assert.That(callbacks, Is.EqualTo(0));
        }

        /*
        CanRemove
        */

        [Test]
        public void CanRemove_ItemNotAdded_ReturnsFalse()
        {
            var inventory = new InventoryManager(1, 1);
            Assert.That(inventory.CanRemove(CreateFullItem(1, 1)), Is.False);
        }

        [Test]
        public void CanRemove_ItemAdded_ReturnsTrue()
        {
            var inventory = new InventoryManager(1, 1);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            Assert.That(inventory.CanRemove(item), Is.True);
        }

        /*
        Remove
        */

        [Test]
        public void Remove_ItemAddedFirst_ItemRemoved()
        {
            var inventory = new InventoryManager(1, 1);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.Remove(item);
            Assert.That(inventory.Contains(item), Is.False);
        }

        [Test]
        public void Remove_ItemNotAdded_OnItemRemovedNotInvoked()
        {
            var inventory = new InventoryManager(1, 1);
            var callbacks = 0;
            inventory.OnItemRemoved += (i) => { callbacks++; };
            inventory.Remove(CreateFullItem(1, 1));
            Assert.That(callbacks, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ItemAdded_OnItemRemovedInvoked()
        {
            var inventory = new InventoryManager(1, 1);
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.OnItemRemoved += (i) =>
            {
                lastItem = i;
                callbacks++;
            };
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            inventory.Remove(item);
            Assert.That(lastItem, Is.SameAs(item));
            Assert.That(callbacks, Is.EqualTo(1));
        }

        /*
        Drop
        */

        [Test]
        public void Drop_ItemNotPresentInInventory_OnItemDroppedInvoked()
        {
            var inventory = new InventoryManager(1, 1);

            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.OnItemDropped += (i) =>
            {
                lastItem = i;
                callbacks++;
            };

            var item = CreateFullItem(1, 1);
            inventory.Drop(item);

            Assert.That(lastItem, Is.SameAs(item));
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void Drop_ItemPresentInInventory_ItemRemoved()
        {
            var inventory = new InventoryManager(1, 1);
            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.Drop(item);
            Assert.That(inventory.Contains(item), Is.False);
        }

        [Test]
        public void Drop_ItemPresentInInventory_OnItemDroppedInvoked()
        {
            var inventory = new InventoryManager(1, 1);

            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.OnItemDropped += (i) =>
            {
                lastItem = i;
                callbacks++;
            };

            var item = CreateFullItem(1, 1);
            inventory.Add(item);
            inventory.Drop(item);

            Assert.That(lastItem, Is.SameAs(item));
            Assert.That(callbacks, Is.EqualTo(1));
        }

        /*
        DropAll
        */

        [Test]
        public void DropAll_AllItemsRemoved()
        {
            var inventory = new InventoryManager(3, 3);
            var item1 = CreateFullItem(1, 1);
            var item2 = CreateFullItem(1, 1);
            var item3 = CreateFullItem(1, 1);
            inventory.Add(item1);
            inventory.Add(item2);
            inventory.Add(item3);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(3));
            inventory.DropAll();
            Assert.That(inventory.AllItems.Count, Is.EqualTo(0));
            Assert.That(inventory.Contains(item1), Is.False);
            Assert.That(inventory.Contains(item2), Is.False);
            Assert.That(inventory.Contains(item3), Is.False);
        }

        [Test]
        public void DropAll_OnItemDroppedInvokedForAllItems()
        {
            var inventory = new InventoryManager(3, 3);

            var droppedItems = new List<IInventoryItem>();
            inventory.OnItemDropped += (i) =>
            {
                droppedItems.Add(i);
            };

            var item1 = CreateFullItem(1, 1);
            var item2 = CreateFullItem(1, 1);
            var item3 = CreateFullItem(1, 1);
            inventory.Add(item1);
            inventory.Add(item2);
            inventory.Add(item3);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(3));

            inventory.DropAll();

            Assert.That(droppedItems.Count, Is.EqualTo(3));
            Assert.That(droppedItems.Contains(item1), Is.True);
            Assert.That(droppedItems.Contains(item2), Is.True);
            Assert.That(droppedItems.Contains(item3), Is.True);
        }

        /*
        Clear
        */

        [Test]
        public void Clear_AllItemsRemoved()
        {
            var inventory = new InventoryManager(3, 3);
            var item = CreateFullItem(3, 3);
            inventory.Add(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.Clear();
            Assert.That(inventory.Contains(item), Is.False);
        }

        [Test]
        public void Clear_OnClearedInvoked()
        {
            var inventory = new InventoryManager(3, 3);
            var callbacks = 0;
            inventory.OnCleared += () => { callbacks++; };
            inventory.Clear();
            Assert.That(callbacks, Is.EqualTo(1));
        }

        /*
        GetAtPoint
        */

        [Test]
        public void GetAtPoint_ItemFound_ReturnsItem()
        {
            var inventory = new InventoryManager(3, 3);
            var item1 = CreateFullItem(2, 2);
            var item2 = CreateFullItem(1, 2);
            var item3 = CreateFullItem(1, 1);
            inventory.Add(item1);
            inventory.Add(item2);
            inventory.Add(item3);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(3));

            Assert.That(inventory.GetAtPoint(new Point(0, 0)), Is.EqualTo(item1));
            Assert.That(inventory.GetAtPoint(new Point(1, 0)), Is.EqualTo(item1));
            Assert.That(inventory.GetAtPoint(new Point(1, 1)), Is.EqualTo(item1));
            Assert.That(inventory.GetAtPoint(new Point(0, 1)), Is.EqualTo(item1));

            Assert.That(inventory.GetAtPoint(new Point(2, 0)), Is.EqualTo(item2));
            Assert.That(inventory.GetAtPoint(new Point(2, 1)), Is.EqualTo(item2));

            Assert.That(inventory.GetAtPoint(new Point(0, 2)), Is.EqualTo(item3));

            Assert.That(inventory.GetAtPoint(new Point(2, 2)), Is.Null);
            Assert.That(inventory.GetAtPoint(new Point(2, 3)), Is.Null);
        }

        [Test]
        public void GetAtPoint_ItemNotFoundReturnsNull()
        {
            var inventory = new InventoryManager(1, 1);
            Assert.That(inventory.GetAtPoint(new Point(1, 1)), Is.Null);
        }

        /*
        GetAtPoint
        */

        [Test]
        public void Resize_WidthAndHeightUpdated()
        {
            var inventory = new InventoryManager(2, 2);
            inventory.Resize(6, 8);
            Assert.That(inventory.Width, Is.EqualTo(6));
            Assert.That(inventory.Height, Is.EqualTo(8));
        }

        [Test]
        public void Resize_OnResizedInvoked()
        {
            var inventory = new InventoryManager(2, 2);
            var callbacks = 0;
            inventory.OnResized += () => { callbacks++; };
            inventory.Resize(3, 3);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void Resize_AllItemFits_NoItemsRemoved()
        {
            var inventory = new InventoryManager(2, 2);
            var item1 = CreateFullItem(1, 1);
            var item2 = CreateFullItem(2, 1);
            var item3 = CreateFullItem(1, 1);
            inventory.Add(item1);
            inventory.Add(item2);
            inventory.Add(item3);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(3));
            inventory.Resize(3, 3);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(3));
            Assert.That(inventory.AllItems.Contains(item1), Is.True);
            Assert.That(inventory.AllItems.Contains(item2), Is.True);
            Assert.That(inventory.AllItems.Contains(item3), Is.True);
        }

        [Test]
        public void Resize_SomeItemsNoLongerFits_ItemsRemoved()
        {
            var inventory = new InventoryManager(3, 3);
            var droppedItems = new List<IInventoryItem>();
            inventory.OnItemDropped += (i) =>
            {
                droppedItems.Add(i);
            };
            var item1 = CreateFullItem(3, 1);
            var item2 = CreateFullItem(1, 1);
            var item3 = CreateFullItem(2, 2);
            inventory.Add(item1);
            inventory.Add(item2);
            inventory.Add(item3);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(3));
            inventory.Resize(2, 2);
            Assert.That(inventory.AllItems.Count, Is.EqualTo(1));
            Assert.That(inventory.AllItems.Contains(item1), Is.False);
            Assert.That(inventory.AllItems.Contains(item2), Is.True);
            Assert.That(inventory.AllItems.Contains(item3), Is.False);
            Assert.That(droppedItems.Count, Is.EqualTo(2));
            Assert.That(droppedItems.Contains(item1), Is.True);
            Assert.That(droppedItems.Contains(item3), Is.True);
        }
    }
}