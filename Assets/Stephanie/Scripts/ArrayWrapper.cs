using UnityEngine;
using System.Collections;

public class ArrayWrapper {

	public string[] array;
	
	public static ArrayWrapper Create(string[] array)
	{
		ArrayWrapper wrapper = new ArrayWrapper();
		wrapper.array = array;
		return wrapper;
	}
	
}
