﻿namespace FunctionalProcessing;

[Serializable]
public class ExecutionException(string message) : Exception(message);