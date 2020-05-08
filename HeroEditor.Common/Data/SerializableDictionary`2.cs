// Decompiled with JetBrains decompiler
// Type: HeroEditor.Common.Data.SerializableDictionary`2
// Assembly: HeroEditor.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9D5B1BE-AD53-47A8-B0FE-D2D5DBF20268
// Assembly location: C:\Users\mnewp\source\repos\Screensaver-Engine\FantasyHeroes\Assets\HeroEditor\Common\HeroEditor.Common.dll


using System;
using System.Collections.Generic;

namespace HeroEditor.Common.Data
{
  [Serializable]
  public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
  {
    [SerializeField]
    protected List<TKey> KeyList = new List<TKey>();
    [SerializeField]
    protected List<TValue> ValueList = new List<TValue>();

    public void OnBeforeSerialize()
    {
      KeyList.Clear();
      ValueList.Clear();
      foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
      {
        KeyList.Add(keyValuePair.Key);
        ValueList.Add(keyValuePair.Value);
      }
    }

    public void OnAfterDeserialize()
    {
      Clear();
      if (KeyList.Count != ValueList.Count)
        throw new Exception(string.Format(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.ag(), KeyList.Count, ValueList.Count));
      for (int index = 0; index < KeyList.Count; ++index)
        Add(KeyList[index], ValueList[index]);
    }
  }
}
