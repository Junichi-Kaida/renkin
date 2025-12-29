using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class MVPSetupTool : EditorWindow
{
    [MenuItem("AlchemyGame/1. Initialize MVP Assets")]
    public static void InitializeAssets()
    {
        string dataPath = "Assets/Data";
        if (!AssetDatabase.IsValidFolder(dataPath)) AssetDatabase.CreateFolder("Assets", "Data");
        if (!AssetDatabase.IsValidFolder(dataPath + "/Materials")) AssetDatabase.CreateFolder(dataPath, "Materials");
        if (!AssetDatabase.IsValidFolder(dataPath + "/Weapons")) AssetDatabase.CreateFolder(dataPath, "Weapons");
        if (!AssetDatabase.IsValidFolder(dataPath + "/Recipes")) AssetDatabase.CreateFolder(dataPath, "Recipes");

        MaterialSO wood = CreateMaterial("Wood", "木材");
        MaterialSO ore = CreateMaterial("Ore", "鉱石");
        MaterialSO resin = CreateMaterial("Resin", "樹脂");

        WeaponSO initial = CreateWeapon("InitialWeapon", "ボロの剣", 1, 0.55f, 1.5f);
        WeaponSO woodSword = CreateWeapon("WoodSword", "木刀", 1, 0.35f, 1.8f);
        WeaponSO steelSword = CreateWeapon("SteelSword", "鍛錬刀", 2, 0.45f, 2.2f);

        CreateRecipe("WoodSwordRecipe", "木刀のレシピ", woodSword, new List<Ingredient> { 
            new Ingredient { material = wood, amount = 2 } 
        });
        CreateRecipe("SteelSwordRecipe", "鍛錬刀のレシピ", steelSword, new List<Ingredient> { 
            new Ingredient { material = wood, amount = 1 },
            new Ingredient { material = ore, amount = 1 }
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        CreateItemPickupPrefab();
        
        Debug.Log("MVP Assets Initialized!");
    }

    private static void CreateItemPickupPrefab()
    {
        string prefabPath = "Assets/Data/ItemPickup.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null) return;

        GameObject obj = new GameObject("ItemPickup_Temp");
        obj.AddComponent<SpriteRenderer>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        obj.GetComponent<SpriteRenderer>().color = Color.yellow;
        obj.AddComponent<CircleCollider2D>().isTrigger = true;
        obj.AddComponent<ItemPickup>();
        
        // Ensure Tag "Player" exists (pre-check)
        obj.layer = LayerMask.NameToLayer("Default");

        PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
        DestroyImmediate(obj);
        Debug.Log("ItemPickup Prefab created!");
    }

    private static void CreateAlchemyUIPrefab()
    {
        string prefabPath = "Assets/Data/AlchemyUI.prefab";
        // Always regenerate to ensure updates are applied
        // if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null) return;

        // Create Canvas structure
        GameObject canvasObj = new GameObject("AlchemyUI_Temp");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.1f);
        rt.anchorMax = new Vector2(0.9f, 0.9f);

        // UI Script
        AlchemyUI ui = canvasObj.AddComponent<AlchemyUI>();

        // Material Text
        GameObject matTextObj = new GameObject("MaterialText");
        matTextObj.transform.SetParent(panel.transform, false);
        Text matText = matTextObj.AddComponent<Text>();
        matText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        matText.color = Color.white;
        matText.alignment = TextAnchor.UpperLeft;
        matText.rectTransform.anchorMin = new Vector2(0, 0.5f);
        matText.rectTransform.anchorMax = new Vector2(0.4f, 1f);
        ui.GetType().GetField("materialDisplayText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, matText);

        // Result Text
        GameObject resTextObj = new GameObject("ResultText");
        resTextObj.transform.SetParent(panel.transform, false);
        Text resText = resTextObj.AddComponent<Text>();
        resText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resText.color = Color.white;
        resText.alignment = TextAnchor.UpperLeft;
        resText.rectTransform.anchorMin = new Vector2(0.45f, 0.2f);
        resText.rectTransform.anchorMax = new Vector2(1f, 0.8f);
        ui.GetType().GetField("resultInfoText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, resText);

        // Craft Button
        GameObject craftBtnObj = new GameObject("CraftButton");
        craftBtnObj.transform.SetParent(panel.transform, false);
        Image craftBg = craftBtnObj.AddComponent<Image>();
        craftBg.color = Color.green;
        Button craftBtn = craftBtnObj.AddComponent<Button>();
        craftBtn.targetGraphic = craftBg;
        craftBtnObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.6f, 0.05f);
        craftBtnObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.15f);
        GameObject craftTextObj = new GameObject("Text");
        craftTextObj.transform.SetParent(craftBtnObj.transform, false);
        Text craftText = craftTextObj.AddComponent<Text>();
        craftText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        craftText.text = "Craft";
        craftText.alignment = TextAnchor.MiddleCenter;
        craftText.color = Color.black;
        craftText.rectTransform.anchorMin = Vector2.zero;
        craftText.rectTransform.anchorMax = Vector2.one;
        ui.GetType().GetField("craftButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, craftBtn);

        // Close Button
        GameObject closeBtnObj = new GameObject("CloseButton");
        closeBtnObj.transform.SetParent(panel.transform, false);
        Image closeBg = closeBtnObj.AddComponent<Image>();
        closeBg.color = Color.red;
        Button closeBtn = closeBtnObj.AddComponent<Button>();
        closeBtn.targetGraphic = closeBg;
        closeBtnObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.92f, 0.92f);
        closeBtnObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 0.98f);
        GameObject closeTextObj = new GameObject("Text");
        closeTextObj.transform.SetParent(closeBtnObj.transform, false);
        Text closeText = closeTextObj.AddComponent<Text>();
        closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        closeText.text = "X";
        closeText.alignment = TextAnchor.MiddleCenter;
        closeText.color = Color.white;
        closeText.rectTransform.anchorMin = Vector2.zero;
        closeText.rectTransform.anchorMax = Vector2.one;
        ui.GetType().GetField("closeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, closeBtn);

        // Recipe Button Prefab setup
        GameObject rBtnAsset = CreateRecipeButtonPrefab();
        ui.GetType().GetField("recipeButtonPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, rBtnAsset);

        // Recipe List Parent
        GameObject listParent = new GameObject("RecipeList");
        listParent.transform.SetParent(panel.transform, false);
        listParent.AddComponent<VerticalLayoutGroup>().childControlHeight = false;
        listParent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        listParent.GetComponent<RectTransform>().anchorMax = new Vector2(0.3f, 0.5f);
        ui.GetType().GetField("recipeListParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, listParent.transform);
        
        // Assign Recipes
        List<RecipeSO> recipes = new List<RecipeSO>();
        string[] guids = AssetDatabase.FindAssets("t:RecipeSO");
        foreach(var g in guids) recipes.Add(AssetDatabase.LoadAssetAtPath<RecipeSO>(AssetDatabase.GUIDToAssetPath(g)));
        ui.GetType().GetField("availableRecipes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ui, recipes);

        PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
        DestroyImmediate(canvasObj);
        // Clean up temp prefab inside prefab if it leaked? No, SaveAsPrefabAsset handles hierarchy.
        Debug.Log("AlchemyUI Prefab created!");
    }

    private static GameObject CreateRecipeButtonPrefab()
    {
        string path = "Assets/Data/RecipeButton.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject rBtnPrefab = new GameObject("RecipeButton");
        Image rBg = rBtnPrefab.AddComponent<Image>();
        rBg.color = Color.gray;
        Button rBtn = rBtnPrefab.AddComponent<Button>();
        rBtn.targetGraphic = rBg;
        
        GameObject rTextObj = new GameObject("Text");
        rTextObj.transform.SetParent(rBtnPrefab.transform, false);
        Text rText = rTextObj.AddComponent<Text>();
        rText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rText.color = Color.black;
        rText.alignment = TextAnchor.MiddleCenter;
        rText.rectTransform.anchorMin = Vector2.zero;
        rText.rectTransform.anchorMax = Vector2.one;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(rBtnPrefab, path);
        DestroyImmediate(rBtnPrefab);
        return prefab;
    }

    private static MaterialSO CreateMaterial(string fileName, string matName)
    {
        MaterialSO mat = ScriptableObject.CreateInstance<MaterialSO>();
        mat.materialName = matName;
        AssetDatabase.CreateAsset(mat, $"Assets/Data/Materials/{fileName}.asset");
        return mat;
    }

    private static WeaponSO CreateWeapon(string fileName, string weaponName, int dmg, float interval, float range)
    {
        WeaponSO w = ScriptableObject.CreateInstance<WeaponSO>();
        w.weaponName = weaponName;
        w.damage = dmg;
        w.attackInterval = interval;
        w.attackRange = range;
        // Try to assign a default Unity sprite
        w.weaponSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        AssetDatabase.CreateAsset(w, $"Assets/Data/Weapons/{fileName}.asset");
        return w;
    }

    private static void CreateRecipe(string fileName, string recipeName, WeaponSO result, List<Ingredient> ingredients)
    {
        RecipeSO r = ScriptableObject.CreateInstance<RecipeSO>();
        r.recipeName = recipeName;
        r.resultWeapon = result;
        r.ingredients = ingredients;
        AssetDatabase.CreateAsset(r, $"Assets/Data/Recipes/{fileName}.asset");
    }

    [MenuItem("AlchemyGame/2. Setup Basic Scene")]
    public static void SetupScene()
    {
        // 0. Clean up existing objects to avoid duplication
        GameObject oldPlayer = GameObject.Find("Player");
        if (oldPlayer != null) DestroyImmediate(oldPlayer);
        GameObject oldFloor = GameObject.Find("Floor");
        if (oldFloor != null) DestroyImmediate(oldFloor);

        // 1. Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0, -1, 0);
        floor.transform.localScale = new Vector3(20, 1, 1);
        
        DestroyImmediate(floor.GetComponent<Collider>());
        floor.AddComponent<BoxCollider2D>();
        
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1) floor.layer = groundLayer;

        // 2. Player
        // 2. Player
        string playerSpritePath = "Assets/Art/Textures/Player.png";
        string playerSideSpritePath = "Assets/Art/Textures/Player_Side.png";
        string playerRun1SpritePath = "Assets/Art/Textures/Player_Run1.png";
        
        AssetDatabase.Refresh(); // Ensure files are seen
        
        // Adjust PPU to match sizes. 
        // Side height is 248 (PPU 150). Run height is 270.
        // To match visual height: 270 / (248/150) = approx 163.
        ImportSprite(playerSpritePath, 512);      // Front
        ImportSprite(playerSideSpritePath, 150);  // Side
        ImportSprite(playerRun1SpritePath, 165);  // Run (Shrink to match Side)

        GameObject player = new GameObject("Player");
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        Sprite pSprite = AssetDatabase.LoadAssetAtPath<Sprite>(playerSpritePath);
        Sprite sSprite = AssetDatabase.LoadAssetAtPath<Sprite>(playerSideSpritePath);
        Sprite rSprite1 = AssetDatabase.LoadAssetAtPath<Sprite>(playerRun1SpritePath);
        
        if (pSprite != null) sr.sprite = pSprite;
        else 
        {
            // Fallback
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            DestroyImmediate(player.GetComponent<Collider>());
        }

        player.tag = "Player";
        player.transform.position = Vector3.zero;
        
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) rb = player.AddComponent<Rigidbody2D>();
        
        if (rb != null) 
        {
            rb.gravityScale = 3f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        player.AddComponent<CapsuleCollider2D>();
        PlayerController pc = player.AddComponent<PlayerController>();
        
        // Assign Sprites
        if (pc != null)
        {
            var frontField = typeof(PlayerController).GetField("frontSprite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (frontField != null && pSprite != null) frontField.SetValue(pc, pSprite);
            
            var sideField = typeof(PlayerController).GetField("sideSprite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (sideField != null && sSprite != null) sideField.SetValue(pc, sSprite);

            var runField = typeof(PlayerController).GetField("runSprites", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (runField != null && rSprite1 != null) 
            {
                Sprite[] runArr = new Sprite[] { rSprite1 };
                runField.SetValue(pc, runArr);
            }
        }

        PlayerCombat pCombat = player.AddComponent<PlayerCombat>();
        player.AddComponent<PlayerHealth>();

        // 3. Helpers (GroundCheck and AttackPoint)
        GameObject gc = new GameObject("GroundCheck");
        gc.transform.SetParent(player.transform);
        gc.transform.localPosition = new Vector3(0, -1.1f, 0);

        GameObject ap = new GameObject("AttackPoint");
        ap.transform.SetParent(player.transform);
        ap.transform.localPosition = new Vector3(1, 0, 0);

        GameObject wv = new GameObject("WeaponVisual");
        wv.transform.SetParent(player.transform);
        wv.transform.localPosition = new Vector3(0.7f, 0, 0);
        wv.transform.localScale = new Vector3(1.5f, 0.2f, 1f); // Make it look like a blade
        SpriteRenderer wvSr = wv.AddComponent<SpriteRenderer>();
        wvSr.sortingOrder = 5; // Ensure it's in front of player

        // 4. Automatic Assignment (Finding private fields via Reflection to bypass [SerializeField])
        if (pc != null)
        {
            var field = typeof(PlayerController).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(pc, gc.transform);
            
            var layerField = typeof(PlayerController).GetField("groundLayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (layerField != null && groundLayer != -1) layerField.SetValue(pc, (LayerMask)(1 << groundLayer));
        }

        if (pCombat != null)
        {
            var apField = typeof(PlayerCombat).GetField("attackPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (apField != null) apField.SetValue(pCombat, ap.transform);

            var wrField = typeof(PlayerCombat).GetField("weaponRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (wrField != null) wrField.SetValue(pCombat, wvSr);

            // Automatically assign Enemy Layer
            var enemyLayerField = typeof(PlayerCombat).GetField("enemyLayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int enemyLayerVal = LayerMask.NameToLayer("Enemy");
            if (enemyLayerField != null && enemyLayerVal != -1) enemyLayerField.SetValue(pCombat, (LayerMask)(1 << enemyLayerVal));

            // Automatically assign InitialWeapon
            WeaponSO initialWeapon = AssetDatabase.LoadAssetAtPath<WeaponSO>("Assets/Data/Weapons/InitialWeapon.asset");
            if (initialWeapon != null)
            {
                var weaponField = typeof(PlayerCombat).GetField("currentWeapon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (weaponField != null) weaponField.SetValue(pCombat, initialWeapon);
            }
        }

        Debug.Log("Scene Setup complete! References assigned automatically.");

        // 5. Setup Enemy Drops
        GameObject itemPickupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Data/ItemPickup.prefab");
        // Update: Slime drops WOOD so player can craft WoodSword first
        MaterialSO defaultDrop = AssetDatabase.LoadAssetAtPath<MaterialSO>("Assets/Data/Materials/Wood.asset");
        
        EnemyBase[] enemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            var prefabField = typeof(EnemyBase).GetField("pickupPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prefabField != null && (GameObject)prefabField.GetValue(enemy) == null) prefabField.SetValue(enemy, itemPickupPrefab);

            var dropField = typeof(EnemyBase).GetField("dropItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // Force set Wood for testing if it's currently null or resin
            if (dropField != null) dropField.SetValue(enemy, defaultDrop);
        }

        Debug.Log("Enemy drops configured (Default: Wood)!");

        // 6. Setup Alchemy Table
        GameObject table = GameObject.Find("AlchemyTable");
        if (table != null) DestroyImmediate(table);
        
        table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "AlchemyTable";
        table.transform.position = new Vector3(-3, 0, 0); // Place to the left
        table.transform.localScale = new Vector3(2, 2, 1);
        DestroyImmediate(table.GetComponent<Collider>());
        
        // Add Trigger
        BoxCollider2D bc = table.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        
        // Add Script
        AlchemyTable at = table.AddComponent<AlchemyTable>();

        // Ensure EventSystem exists for UI
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Assign UI Prefab
        CreateAlchemyUIPrefab(); // Ensure prefab exists
        GameObject uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Data/AlchemyUI.prefab");
        var uiField = typeof(AlchemyTable).GetField("alchemyUIPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (uiField != null) uiField.SetValue(at, uiPrefab);

        // Ensure InventoryManager exists and add Debug Wood
        InventoryManager invMgr = Object.FindAnyObjectByType<InventoryManager>();
        if (invMgr == null)
        {
             invMgr = new GameObject("InventoryManager").AddComponent<InventoryManager>();
        }
        
        MaterialSO wood = AssetDatabase.LoadAssetAtPath<MaterialSO>("Assets/Data/Materials/Wood.asset");
        if (wood != null)
        {
            var matListField = typeof(InventoryManager).GetField("startingMaterials", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var amtListField = typeof(InventoryManager).GetField("startingAmounts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (matListField != null && amtListField != null)
            {
                var matList = new List<MaterialSO> { wood };
                var amtList = new List<int> { 10 }; // Give 10 Wood
                matListField.SetValue(invMgr, matList);
                amtListField.SetValue(invMgr, amtList);
            }
        }
        
        // Ensure AlchemyManager exists
        if (Object.FindAnyObjectByType<AlchemyManager>() == null)
        {
             new GameObject("AlchemyManager").AddComponent<AlchemyManager>();
        }

        Debug.Log("Alchemy Table & Managers configured!");
        Debug.Log("TIP: If you see Input System errors, go to Edit > Project Settings > Player > Other Settings > Active Input Handling and set it to 'Both'.");
    }
    private static void ImportSprite(string path, int ppu)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            // 1. Setup global importer settings
            importer.textureType = TextureImporterType.Sprite;
            
            // 2. Read current settings into object
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);

            // 3. Modify settings object
            settings.spritePixelsPerUnit = ppu;
            settings.filterMode = FilterMode.Point;
            settings.alphaIsTransparency = true;
            settings.spriteMode = (int)SpriteImportMode.Single;
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            
            // 4. Apply settings back
            importer.SetTextureSettings(settings);
            
            // 5. Apply other properties that might not be in Settings or require explicit set
            importer.compressionQuality = 0; // High quality

            importer.SaveAndReimport();
        }
    }
}
