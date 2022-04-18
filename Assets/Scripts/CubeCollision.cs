using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeCollision : MonoBehaviour
{
    private Cube cube;

    private void Awake()
    {
        cube = GetComponent<Cube>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Cube otherCube = collision.gameObject.GetComponent<Cube>();

        //check if contacted with other cube

        if (otherCube != null && cube.CubeID > otherCube.CubeID)
        {
            if (otherCube.CubeNumber == cube.CubeNumber)
            {
                Vector3 contactPoint = collision.contacts[0].point;

                if (otherCube.CubeNumber < CubeSpawner.Instance.maxCubeNumber)
                {
                    Cube newCube = CubeSpawner.Instance.Spawn(cube.CubeNumber * 2, contactPoint + Vector3.up * 1.6f);

                    float pushForce = 2.5f;
                    newCube.CubeRigidbody.AddForce(new Vector3(0, .3f, 1f) * pushForce, ForceMode.Impulse);

                    float randomValue = Random.Range(-20, 20);
                    Vector3 randomDirection = Vector3.one * randomValue;
                    newCube.CubeRigidbody.AddTorque(randomDirection);
                }

                //the explosion should affect surrounded cubes too:

                Collider[] surreoundedCubes = Physics.OverlapSphere(contactPoint, 2f);
                float explosionForce = 100f;
                float explosionRadius = 1.5f;

                foreach (Collider coll in surreoundedCubes)
                {
                    if (coll.attachedRigidbody != null)
                    {
                        coll.attachedRigidbody.AddExplosionForce(explosionForce, contactPoint, explosionRadius);
                    }
                }
                
                FX.Instance.PlayCubeExplosionFX(contactPoint,cube.CubeColor);

                CubeSpawner.Instance.DestroyCube(cube);
                CubeSpawner.Instance.DestroyCube(otherCube);
            }
        }
    }
}