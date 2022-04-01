﻿/**
 * LabeledArrayAttribute.cs
 * Created by: Joao Borks [joao.borks@gmail.com]
 * Created on: 28/12/17 (dd/mm/yy)
 * Reference from John Avery: https://forum.unity.com/threads/how-to-change-the-name-of-list-elements-in-the-inspector.448910/
 */

using UnityEngine;
using System;

namespace Util
{

    public class LabeledArrayAttribute : PropertyAttribute
    {
        public LabeledArrayAttribute() { }
    }

    public interface INamedArrayElement
    {
        string displayName { get; }
    }
}