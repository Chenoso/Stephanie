﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Config : MonoBehaviour {

	public static string masterURL = "http://clientes.cheny.com.br/stephanie/es2.php";
	public static string masterUser = "cheny";
	public static string masterPass = "chenyrox";

	public static string user = "TestUser";

	public static int childNumber = 0;
	public static List<ArrayWrapper> childNames = new List<ArrayWrapper>();
	public static List<Texture2D> childTextures2D = new List<Texture2D>();

	public static string currentChildName;
	public static string currentChild;
	public static int currentChildDayMemoriesCount;
	public static List<ArrayWrapper> currentChildDatesCalendarList = new List<ArrayWrapper>();
	public static List<ArrayWrapper> currentChildDayMemoriesList = new List<ArrayWrapper>();
	public static List<Texture2D> currentChildDayMemoriesPhotoList = new List<Texture2D>();
	public static List<Texture2D> currentChildDayMemoriesThumbList = new List<Texture2D>();

	public static string currentDay;
	public static string currentMonth;
	public static string currentYear;

	public static int currentMemoryIndex;
	public static string currentMemoryDay;
	public static string currentMemoryMonth;
	public static string currentMemoryYear;
	public static string currentMemoryTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
