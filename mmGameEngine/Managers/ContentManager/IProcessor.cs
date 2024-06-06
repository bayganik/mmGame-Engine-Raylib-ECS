using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine;

public interface IProcessor
{
    //
    //Interface for all content to be processed
    //
    object Load<T>(string path);
    void Unload(object texture);
}
