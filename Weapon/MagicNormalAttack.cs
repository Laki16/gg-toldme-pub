﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TVNT
{
    public class MagicNormalAttack : WeaponController
    {
        void Update()
        {
            transform.Translate(new Vector3(10, 3, 0) * Time.deltaTime);
            //빗나갔을 경우 2초 후 삭제
            GameObject.Destroy(gameObject, 2.0f);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Monster")
            {
                Debug.Log("magic attack!!");
                if (other.GetComponent<TVNTCharacterController>().lives > 0)
                {
                    other.GetComponent<TVNTCharacterController>().lives -= damage;
                    other.GetComponent<MonsterAIController>().threatenTime = 0;
                }
                else
                {
                    //other.GetComponent<MonsterAIController>().CharacterDead();
                    other.gameObject.SetActive(false);
                }
                GameObject.Destroy(gameObject);
            }
        }

    }
}