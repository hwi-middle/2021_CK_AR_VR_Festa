using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogCSVReader
{
	public class Row
	{
		public string id;
		public string speaker;
		public string dialog;
		public string speed;
		public string time;
		public string reply1;
		public string reply2;
		public string goto1;
		public string goto2;

	}

	List<Row> rowList = new List<Row>();
	bool isLoaded = false;

	public bool IsLoaded()
	{
		return isLoaded;
	}

	public List<Row> GetRowList()
	{
		return rowList;
	}

	public void Load(TextAsset csv)
	{
		rowList.Clear();
		string[][] grid = CsvParser2.Parse(csv.text);
		for(int i = 1 ; i < grid.Length ; i++)
		{
			Row row = new Row();
			row.id = grid[i][0];
			row.speaker = grid[i][1];
			row.dialog = grid[i][2];
			row.speed = grid[i][3];
			row.time = grid[i][4];
			row.reply1 = grid[i][5];
			row.reply2 = grid[i][6];
			row.goto1 = grid[i][7];
			row.goto2 = grid[i][8];

			rowList.Add(row);
		}
		isLoaded = true;
	}

	public int NumRows()
	{
		return rowList.Count;
	}

	public Row GetAt(int i)
	{
		if(rowList.Count <= i)
			return null;
		return rowList[i];
	}

	public Row Find_id(string find)
	{
		return rowList.Find(x => x.id == find);
	}
	public List<Row> FindAll_id(string find)
	{
		return rowList.FindAll(x => x.id == find);
	}
	public Row Find_speaker(string find)
	{
		return rowList.Find(x => x.speaker == find);
	}
	public List<Row> FindAll_speaker(string find)
	{
		return rowList.FindAll(x => x.speaker == find);
	}
	public Row Find_dialog(string find)
	{
		return rowList.Find(x => x.dialog == find);
	}
	public List<Row> FindAll_dialog(string find)
	{
		return rowList.FindAll(x => x.dialog == find);
	}
	public Row Find_speed(string find)
	{
		return rowList.Find(x => x.speed == find);
	}
	public List<Row> FindAll_speed(string find)
	{
		return rowList.FindAll(x => x.speed == find);
	}
	public Row Find_time(string find)
	{
		return rowList.Find(x => x.time == find);
	}
	public List<Row> FindAll_time(string find)
	{
		return rowList.FindAll(x => x.time == find);
	}
	public Row Find_reply1(string find)
	{
		return rowList.Find(x => x.reply1 == find);
	}
	public List<Row> FindAll_reply1(string find)
	{
		return rowList.FindAll(x => x.reply1 == find);
	}
	public Row Find_reply2(string find)
	{
		return rowList.Find(x => x.reply2 == find);
	}
	public List<Row> FindAll_reply2(string find)
	{
		return rowList.FindAll(x => x.reply2 == find);
	}
	public Row Find_goto1(string find)
	{
		return rowList.Find(x => x.goto1 == find);
	}
	public List<Row> FindAll_goto1(string find)
	{
		return rowList.FindAll(x => x.goto1 == find);
	}
	public Row Find_goto2(string find)
	{
		return rowList.Find(x => x.goto2 == find);
	}
	public List<Row> FindAll_goto2(string find)
	{
		return rowList.FindAll(x => x.goto2 == find);
	}

}