using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VRInventory : MonoBehaviour
{
    public bool IsStatic; //If true the grid won't update automatically in game when the item set changes
    Vector2 dimentions; //The dimentions of the parent panel
    public Vector2 distMul; //Distance Multiplier, the distance between elements 
    RectTransform plane;
    RectTransform can;
    int ItemNum; 
    int DisplayedNum = 0;
    public int RowElemNum; //Number of elements in a row

                                                   /************** ALIGNMENT **************/
    public enum alignX 
    {
        Center,
        Left,
        Right
    };
    public enum alignY
    {
        Center,
        Top,
        Bottom
    };
    public alignX AlignmentX;
    public alignY AlignmentY;

                        //Dunno why I did that, btw end of alignment things. It's fairly straight forward anyway

    public float Padding; //Distance from sides of panel
    public List<GameObject> Items; //Items to be disposed in a grid

    int alX;
    int alY;

    void Start()
    {
        if (!IsStatic) PrepareDisplay();
    }

    void Update()
    {
        if (!IsStatic) Display();
    }

    public void EditorApply() //Called by pressing a button in the editor.
    {

        PrepareDisplay();
        Display();

    }
    void PrepareDisplay()
    {
        Refresh(); //The Items are destroyed, DisplayedNum is resetted.
        ItemNum = Items.ToArray().Length;
        plane = GetComponent<RectTransform>();
        can = transform.parent.GetComponent<RectTransform>();

        dimentions = new Vector2(plane.rect.xMax, plane.rect.yMax);

        switch (AlignmentX) //sets AlX to 0, 1 or 2 depending on the state of AlignmentX
        {
            case alignX.Center:
                alX = 0;
                break;
            case alignX.Left:
                alX = 1;
                break;
            case alignX.Right:
                alX = 2;
                break;
        }
        switch (AlignmentY) //Same with AlY and AlignmentY
        {
            case alignY.Center:
                alY = 0;
                break;
            case alignY.Top:
                alY = 1;
                break;
            case alignY.Bottom:
                alY = 2;
                break;
        }
    }
    void Display() //Displays the Grid
    {
        if (ItemNum != Items.ToArray().Length) ItemNum = Items.ToArray().Length; //Makes sure ItemNum and the number of items is the same

        if (DisplayedNum != ItemNum) //If the number of displayed items differs from the number of items:
        {
            Refresh(); //The Items are destroyed, DisplayedNum is resetted.
            RemoveNull(); //The null items are removed
            CreateItems(); //The items are instantiated
        }
    }

    void RemoveNull() //Removes null elements of Items: ReCurSive fUnctioN oH nOOO!!
    {
        int i = 0;
        foreach (GameObject g in Items) 
        {
            if (g == null)
            {
                //print("Null: " + i.ToString());
                Items.Remove(g);
                RemoveNull();
                break;
            }
            i++;
        }
        return;

    }

    void CreateItems() //Instantiates the Items in a grid
    {
        foreach (GameObject g in Items)
        {
            GameObject newItem = Instantiate(g, transform);

            switch (alY) //Takes Care of alignment of the previously instantiated elements
            {
                case 0:
                    newItem.GetComponent<RectTransform>().localPosition = Centered();
                    break;
                case 1:
                    newItem.GetComponent<RectTransform>().localPosition = Top();
                    break;
                case 2:
                    newItem.GetComponent<RectTransform>().localPosition = Bottom();
                    break;
            }
           
            DisplayedNum++;
        }
    }

    Vector2 ToGridPos(int i) //Returns the position on a grid given the index on the array
    {
        int row = i / RowElemNum;
        int col = i % RowElemNum;
        return new Vector2(col, -row); //row is negative because we want the items to be disposed from left to right and *top to bottom*
    }

    int ColNum() //Returns the number of columns
    {
        if (Items.ToArray().Length % RowElemNum == 0)
            return Items.ToArray().Length / RowElemNum;
        else
            return Items.ToArray().Length / RowElemNum + 1;
    }

    public void Refresh() //Destroyes the items and resets DisplayedNum. It's more of a Reset than a Refresh now that I think about it.
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        DisplayedNum = 0;
    }


    Vector2 Centered() //Align centered on the Y axis
    {
        switch (alX) //takes care of the alignment on the X axis
        {
            case 0: //case centered
                return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x / 2, -(ColNum() - 1) * distMul.y / 2); //It's here that the ma(gic)th happens. This is: the grid
            case 1: //case left
                return ToGridPos(DisplayedNum) * distMul - new Vector2(dimentions.x - Padding, -(ColNum() - 1) * distMul.y / 2);
            case 2: //case right
                return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x - dimentions.x + Padding, -(ColNum() - 1) * distMul.y / 2);
        }
        return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x / 2, -(ColNum() - 1) * distMul.y / 2);

    }

    //same as before but for top and bottom. Have fun!
    Vector2 Top()
    {
        switch (alX)
        {
            case 0:
                return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x / 2, -dimentions.y + Padding);
            case 1:
                return ToGridPos(DisplayedNum) * distMul - new Vector2(dimentions.x - Padding, -dimentions.y + Padding);
            case 2:
                return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x - dimentions.x + Padding, -dimentions.y + Padding);
        }
        return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x / 2,  - dimentions.y + Padding);
    }
    Vector2 Bottom()
    {
        switch (alX)
        {
            case 0:
                return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x / 2, dimentions.y - (ColNum() - 1) * distMul.y - Padding);
            case 1:
                return ToGridPos(DisplayedNum) * distMul - new Vector2(dimentions.x - Padding, dimentions.y - (ColNum() - 1) * distMul.y - Padding);
            case 2:
                return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x - dimentions.x + Padding, dimentions.y - (ColNum() - 1) * distMul.y - Padding);
        }
        return ToGridPos(DisplayedNum) * distMul - new Vector2((RowElemNum - 1) * distMul.x / 2, dimentions.y - (ColNum() - 1) * distMul.y - Padding);
    }

    
}
