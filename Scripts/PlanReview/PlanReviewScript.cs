

string featurePlanPath = args[0];

if (!File.Exists(featurePlanPath))
{
    Console.WriteLine($"[plan-review] plan not found: {featurePlanPath}");
    return;
}

