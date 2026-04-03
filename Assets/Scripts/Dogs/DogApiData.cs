using System;
using System.Collections.Generic;

namespace Dogs
{
    // ── dogapi.dog/api/v2 JSON-модели ──────────────────────────────────────────

    [Serializable]
    public class BreedsResponse
    {
        public List<BreedData> data;
    }

    [Serializable]
    public class BreedData
    {
        public string id;
        public BreedAttributes attributes;
    }

    [Serializable]
    public class BreedAttributes
    {
        public string name;
        public string description;
    }

    [Serializable]
    public class BreedDetailResponse
    {
        public BreedData data;
    }
}
