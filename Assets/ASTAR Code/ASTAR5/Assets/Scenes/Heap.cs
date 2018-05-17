using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Heap optimization: only have to look for parent if greater or not, we cut out the need to compare to all other nodes
//Formula to find any parent in the heap is (n-1)/2
//Formula for child: 2n+1 = child on the left , 2n+2 = child on the right
//Generic so it can work with every class
public class Heap<T> where T : IHeapItem<T> {

    //Instead an array of nodes we make an Array of Type T
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        //difficult to resize so we multiply grid size x with grid size y to get maximum of nodes at any given time
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        //add item to the end of the array, will get sorted later anyways
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        //Take the Item of the end of the heap and put it on the first place
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    //in case if we want to change the priority of an item, example: we find a new path node with lower fCost so we can update the position in the heap as higher priority
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    //number of items currently in the Heap
    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    //check if the heap contains a specific item
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    //Sort after deletion
    void SortDown(T item)
    {
        //Get the intices of the items of the two childrend
        while (true)
        {
            //Formula for child: 2n+1 = child on the left , 2n+2 = child on the right
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            //if item has at least one child (child left)
            if (childIndexLeft < currentItemCount) { 
                swapIndex = childIndexLeft;

                if(childIndexRight < currentItemCount)
                {
                    //check which of the two children has higher priority and swap the index to that child
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                //check if the parent has a lower priority with the highest priority child, if so swap
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                //if the parent is in the correct position exit
                else
                {
                    return;
                }
            }

            //also if we discover that the parent doesnt have any children, its correct position and exit
            else
            {
                return;
            }
        }
    }

    //Takes care of Heap sorting
    void SortUp(T item) {
        //Formular for parent index
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {            
            T parentItem = items[parentIndex];
            //Compare the current Item with the parent item -> use IComparable

            if(item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            //As soon it is no longer higher priority than the parent item -> break out of the loop
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    //if it got a higher priority than return 1 (lower fCost) than swap with parent item, if it gots the same priority return 0, if it gots lower priority return -1
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

}

//leet keep track of every item of its own index in his heap, compare each item to see which one has the higher priority
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
