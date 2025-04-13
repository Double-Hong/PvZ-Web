using System;

public interface IConfigPathProvider
{
    string GetPath(Type configType);
}