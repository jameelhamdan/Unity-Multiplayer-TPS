using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class Inventory : MonoBehaviour
{

    public List<Item> MyInventory = new List<Item>();
    public List<Recipe> MyRecipes = new List<Recipe>();

    public int Inventory_Capacity;
	public GameObject Player_Canvas;
	public GameObject InventoryPanel;


    public GameObject ItemPrefab;
	public GameObject RecipePrefab;
	public int SelectedRecipe = 0;

	void Start()
    {
		Player_Canvas = GameObject.FindGameObjectWithTag("UI/Player_Canvas");
		InventoryPanel = Player_Canvas.transform.Find("Inventory").gameObject;
		//Get Prefabs from Resources
		ItemPrefab = Resources.Load<GameObject>("UI/Inventory/Item"); 
		RecipePrefab = Resources.Load<GameObject>("UI/Inventory/Recipe"); 
		
		GetItemsfromAssets();
		RenderInventoryitems();
		GetRecipesfromAssets();
		RenderCraftingRecipes();
		SelectCraftingRecipe(0);
	}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){

			this.AddItem(3, 1);
        }

		if (Input.GetKeyDown(KeyCode.R))
		{
			this.RemoveItem(3, 1);
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			InventoryPanel.SetActive(!InventoryPanel.activeSelf);
		}

    }

	void GetItemsfromAssets()
	{
		//Get All Objects inside folder
		string folderpath = "Items";
		var tempObject = Resources.LoadAll<Item>(folderpath);

		foreach (var a in tempObject)
		{
			MyInventory.Add(a);
		}
	}


	void RenderInventoryitems()
    {        
        var list = MyInventory;
        GameObject ItemParent = InventoryPanel.transform.Find("Items/Viewport/Content").gameObject;
        //Destroy Object and children and create another Object
        if(ItemParent != null){
            for(int j=0;j<ItemParent.transform.childCount;j++){
                Destroy(ItemParent.transform.GetChild(j).gameObject);
            }
        }

        //View Values
        int i=0; // do some magic
        
        foreach (var item in list)
        {            
            var newItem = Instantiate(ItemPrefab,new Vector3(0,0,0),new Quaternion(0,0,0,0));
            newItem.transform.parent = ItemParent.transform;

            newItem.name = "ItemBox" + i;
            //Edit GUI Values
            newItem.transform.Find("ID").transform.GetComponent<TextMeshProUGUI>().SetText("" + item.Quantity);
            newItem.transform.Find("NAME").transform.GetComponent<TextMeshProUGUI>().SetText(item.Name);
            i++;
        }
    }

    public bool AddItem(int ID, int Quantity = 1)
    {
        try
        {
            var item = MyInventory.Find(x => x.ID == ID);
            if (item.StackMax == item.Quantity)
            {
                Debug.Log("Stack Full");
                return false;
            }
            item.Quantity += Quantity;
        }
        catch
        {
            Debug.Log("Item Not Found");
            return false;
        }
		
        //Render Items Again
        RenderInventoryitems();
		return true;
	}

    public void RemoveItem(int ID, int Num)
    {
		try
		{
			var item = MyInventory.Find(x => x.ID == ID);

			if (item.Quantity > 0)
			{
				item.Quantity -= Num;
			}
			else
			{
				Debug.Log("No More Items to delete");
			}

		}
		catch
		{
			Debug.Log("Item Not Found");
		}

		//Render Items Again
		RenderInventoryitems();
    }

	/*--------------------------------------------------------------------------*/
	/*--------------------------------- Crafting -------------------------------*/
	/*--------------------------------------------------------------------------*/

	void GetRecipesfromAssets()
	{
		//Get All Objects inside folder
		string folderpath = "Recipes";
		var tempObject = Resources.LoadAll<Recipe>(folderpath);

		foreach (var a in tempObject)
		{
			MyRecipes.Add(a);
		}
	}
	void RenderCraftingRecipes()
	{
		var list = MyRecipes;
		GameObject CraftingParent = InventoryPanel.transform.Find("Crafting/RecipeView/Viewport/Content").gameObject;

		//Destroy Object and children and create another Object
		if (CraftingParent != null)
		{
			for (int j = 0; j < CraftingParent.transform.childCount; j++)
			{
				Destroy(CraftingParent.transform.GetChild(j).gameObject);
			}
		}

		int i = 0;
		foreach (var item in list)
		{
			var newItem = Instantiate(RecipePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
			newItem.transform.parent = CraftingParent.transform;
			newItem.name = "RecipeBox" + i; i++;
			//Edit GUI Values
			var result = MyInventory.Find(x => x.ID == item.ResultID);
			newItem.transform.Find("ID").transform.GetComponent<TextMeshProUGUI>().SetText("+" + item.ResultQuantity);
			newItem.transform.Find("NAME").transform.GetComponent<TextMeshProUGUI>().SetText(result.Name);
			
			//Adding Listener
			//newItem.GetComponent<Button>().onClick.AddListener(delegate {this.SelectCraftingRecipe(item.ID); });

		}
	}

	public void SelectCraftingRecipe(int recipeID)
	{
		TextMeshProUGUI RecipeFor = InventoryPanel.transform.Find("Crafting").transform.Find("RecipeForLabel").GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI RequirmentsContent = InventoryPanel.transform.Find("Crafting").transform.Find("RequirementsContent").GetComponent<TextMeshProUGUI>();
		if (recipeID == 0)
		{
			RecipeFor.SetText("Select a Recipe");
			RequirmentsContent.SetText("");
			return;
		}

		var myRecipe = MyRecipes.Find(x => x.ID == recipeID);
		var resultItem = MyInventory.Find(x => x.ID == myRecipe.ResultID);
		RecipeFor.SetText("Recipe For " + myRecipe.ResultQuantity + "x " + resultItem.Name);
		string RequirementsString = "";
		for (int i = 0; i < myRecipe.RequiredItemIds.Length; i++)
		{
			var aItem = MyInventory.Find(x=>x.ID == myRecipe.RequiredItemIds[i]);
			RequirementsString = RequirementsString + myRecipe.RequiredItemQuantaties[i] + "x " + aItem.Name + " \n";
		}
		RequirmentsContent.SetText(RequirementsString);
		SelectedRecipe = recipeID;
	}

	//Recipe ID
	public void Craft()
	{
		int recipeID = SelectedRecipe;
		TextMeshProUGUI RecipeFor = InventoryPanel.transform.Find("Crafting").transform.Find("RecipeForLabel").GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI RequirmentsContent = InventoryPanel.transform.Find("Crafting").transform.Find("RequirementsContent").GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI ErrorMessage = InventoryPanel.transform.Find("Crafting").transform.Find("ErrorMessage").GetComponent<TextMeshProUGUI>();

		var myRecipe = MyRecipes.Find(x => x.ID == recipeID);
		if(myRecipe == null)
		{
			ErrorMessage.SetText("Recipe Not Found");
			return;
		}

		//Array of the required items
		var reqarr = myRecipe.RequiredItemIds;
		var reqarrq = myRecipe.RequiredItemQuantaties;

		for (int i =0; i < reqarr.Length; i++)
		{
			//get how many do i have of item
			var item = MyInventory.Find(x => x.ID == reqarr[i]);
			if (item == null)
			{
				ErrorMessage.SetText("Item Not Found");
				return;
			}
			Debug.Log(item.Quantity + "///" + reqarrq[i]);
			if(item.Quantity < reqarrq[i])
			{
				ErrorMessage.SetText("Not Enough of " + item.Name + " Need at least " + reqarrq[i]);
				return;
			}
		}
		//ALSO CHECK IF THERE IS ROOM IN INVENTORY



		//End of Recipe Validation

		//Start Actually Crafting
		for (int i = 0; i < reqarr.Length; i++)
		{
			//get how many do i have of item
			var item = MyInventory.Find(x => x.ID == reqarr[i]);
			item.Quantity -= reqarrq[i];
		}

		//Add Item to my Inventory 
		ErrorMessage.SetText("Crafted Item");
		MyInventory.Find(x => x.ID == myRecipe.ResultID).Quantity += myRecipe.ResultQuantity;
		RenderInventoryitems();
		RenderCraftingRecipes();
		SelectCraftingRecipe(0);
	}


}
