
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_ArrayWrapper : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		ArrayWrapper data = (ArrayWrapper)obj;
		// Add your writer.Write calls here.
		writer.Write(data.array);

	}
	
	public override object Read(ES2Reader reader)
	{
		ArrayWrapper data = new ArrayWrapper();
		Read(reader, data);
		return data;
	}
	
	public override void Read(ES2Reader reader, object c)
	{
		ArrayWrapper data = (ArrayWrapper)c;
		// Add your reader.Read calls here to read the data into the object.
		data.array = reader.ReadArray<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_ArrayWrapper():base(typeof(ArrayWrapper)){}
}
