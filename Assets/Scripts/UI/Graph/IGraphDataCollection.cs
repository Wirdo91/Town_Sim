using System;
using System.Collections.Generic;

public interface IGraphDataCollection
{
	List<string> GetIdentifiers(Action<IGraphDataCollection, string> onDataChanged);

	(GraphType gType, Dictionary<float, float> data) GetData(string identifier);
}
