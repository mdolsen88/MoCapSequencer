﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.GUI
{
    public interface tabInterface
    {
        void Destroy();
        void UpdateGUI();
        string[] IsReady();
    }
}
