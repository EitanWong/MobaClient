﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtObject
{
    void SendHurtRequest(int HurtValue, int ObjectID);
}