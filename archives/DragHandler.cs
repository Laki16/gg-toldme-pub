﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace TVNT
{

    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameObject prefab;
        GameObject hoverPrefab;

        public GameObject monsterController;
        int mapLayer;
        // Use this for initialization
        void Start()
        {
            monsterController = GameObject.FindGameObjectWithTag("MonsterController");
            hoverPrefab = Instantiate(prefab);
            //RemoveScriptsFromPrefab ();
            AdjustPrefabAlpha();
            hoverPrefab.SetActive(false);
            mapLayer = LayerMask.GetMask("Map");
        }

        //void RemoveScriptsFromPrefab() {
        //	Component[] components = hoverPrefab.GetComponentsInChildren<TurretTargettingSystem>();
        //	foreach (Component component in components) {
        //		Destroy (component);
        //	}
        //}

        void AdjustPrefabAlpha()
        {
            MeshRenderer[] meshRenderers = hoverPrefab.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                Material mat = meshRenderers[i].material;
                meshRenderers[i].material.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            // Debug.Log("Beginning drag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Debug.Log(eventData);
            RaycastHit[] hits;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, 150f, 1<<8);
            //Debug.DrawLine(ray.origin, ray.direction, Color.blue, 150f);

            //Debug.DrawLine(Input.mousePosition + new Vector3(0, 30, 0), Input.mousePosition + new Vector3(0, -20, 0));
            if (hits != null && hits.Length > 0)
            {
                int terrainCollderQuadIndex = GetTerrainColliderQuadIndex(hits);
                if (terrainCollderQuadIndex != -1)
                {
                    hoverPrefab.transform.position = hits[terrainCollderQuadIndex].transform.position + new Vector3(0, 2, 0);
                    hoverPrefab.SetActive(true);
                    // Debug.Log (hits [terrainCollderQuadIndex].point);
                }
                else
                {
                    hoverPrefab.SetActive(false);
                }
            }
        }

        int GetTerrainColliderQuadIndex(RaycastHit[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.name.Equals("Base"))
                {
                    RaycastHit minimapRay;
                    if (Physics.Raycast(hits[i].transform.position + new Vector3(0, -20, 0), hits[i].transform.position + new Vector3(0, -100, 0), out minimapRay, mapLayer))
                    { 
                        if (minimapRay.transform.tag == "Map")
                        {
                            if (!minimapRay.transform.gameObject.GetComponent<MinimapCheck>().isHero)
                            {
                                return i;
                            }
                        }
                    }
                    //return i;
                }
            }
            return -1;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // If the prefab instance is active after dragging stopped, it means
            // it's in the arena so (for now), just drop it in.
            //GameObject clone;
            if (hoverPrefab.activeSelf)
            {
                //clone = Instantiate(prefab, hoverPrefab.transform.position, Quaternion.identity);
                monsterController.GetComponent<MonsterController>().DeployMonster(prefab, hoverPrefab.transform.position, hoverPrefab.transform.rotation);
                //clone.SetActive(true);
            }

            // Then set it to inactive ready for the next drag!
            hoverPrefab.SetActive(false);
        }
    }
}