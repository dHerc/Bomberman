﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
//! Klasa odpowiedzialna za sterowanie postacia przy uzyciu strzalek
public class ArrowsController : MonoBehaviour
{
  public CharacterController characterController; //<! kontroler postaci Unity
  public GameObject bombPrefab;                   //<! prefab bomby
  int cooldown = 0;                               //<! opoznienie w stawianiu bomby
  public Animator animator;                       //<! animator postaci
  public Camera camera;                           //<! kamera
  private Character character;                    //<! logika postaci
  private void Awake()
  {
    // inicjalizacja logiki postaci
    character = gameObject.GetComponent<Character>();
  }

  /** Funkcja do obsługi ruchu postaci*/
  private void Move()
  {
    // sprawdzenie czy postac nie jest w trakcie animacja stawiania bomby
    if (animator.GetBool("isPlanting") == true)
      return;

    // inicjalizcja zmiennej czytającej klawiature
    var keyboard = Keyboard.current;

    // ustalenie zmiennej ruchu poziomego w zależności od tego czy strzalka w prawo i lewo jest wciesnieta
    float horizontalMove = 0;
    if (keyboard.rightArrowKey.isPressed)
      horizontalMove += 1;
    if (keyboard.leftArrowKey.isPressed)
      horizontalMove -= 1;

    // ustalenie zmiennej ruchu pionowego w zależności od tego czy strzalka w gore i dol jest wciesnieta
    float verticalMove = 0;
    if (keyboard.upArrowKey.isPressed)
      verticalMove += 1;
    if (keyboard.downArrowKey.isPressed)
      verticalMove -= 1;

    // inicjalizcja wektora ruchu przy użyciu ustalonych zmiennych
    Vector3 move = new Vector3(1f * horizontalMove, 0f, 1f * verticalMove);

    // okreslenie rotacji
    if (move != Vector3.zero)
      transform.rotation = Quaternion.LookRotation(move) * Quaternion.Euler(0f, -90f, 0f);
    // "symulacja" grawitacji do sciagania gracza w dol po wejsciu na bombe
    move += transform.up * -1;
    // wykonanie ruchu przy uzyciu CharacterController
    characterController.Move(speed * Time.deltaTime * move);
    // przesuniecie kamery
    camera.transform.position = this.transform.position + new Vector3(0, 9, 0);
    // wlaczenie animacji chodzenia
    animator.SetBool("isWalking", horizontalMove != 0 || verticalMove != 0);
  }

  /** Funckja do obsługi stawiania bomby*/
  private void Bomb()
  {
    // inicjalizcja zmiennej czytającej klawiature
    var keyboard = Keyboard.current;
    // jezeli wcisniety jest prawy alt, nie ma opoznienia i postac ma bomby
    if (keyboard.rightAltKey.isPressed && cooldown == 0 && character.bombs > 0)
    {
      // wyslanie do logiki postaci informacji o postawieniu bomby
      character.placeBomb();
      // wlaczenie animacji stawiania bomby
      animator.SetBool("isPlanting", true);
      // ustawienie opoznienia na ok. czas stawiania bomby
      cooldown = 150;
    }
    // ok. polowa czasu stawiania bomby
    if (cooldown == 75)
    {
      int range = character.range;
      int bombLifetime = character.bombLifetime;
      Vector3Int intVector = new Vector3Int((int)this.transform.position.x, (int)this.transform.position.y, (int)this.transform.position.z);
      Vector3 bombPlacement = intVector + new Vector3(1 - (intVector.x) % 2, 1, 1 - (intVector.z) % 2);
      GameObject bomb = Instantiate(bombPrefab, bombPlacement, Quaternion.identity);
      bomb.GetComponent<Bomb>().maxLifetime = bombLifetime;
      bomb.GetComponent<Rigidbody>().velocity = this.gameObject.transform.forward * 0;
      bomb.transform.GetChild(1).GetComponent<BoxCollider>().size = new Vector3(4 * range, 1, 1);
      bomb.transform.GetChild(1).GetComponent<BombExplosion>().lifetime = bombLifetime;
      bomb.transform.GetChild(2).GetComponent<BoxCollider>().size = new Vector3(1, 4 * range, 1);
      bomb.transform.GetChild(2).GetComponent<BombExplosion>().lifetime = bombLifetime;
      bomb.transform.GetChild(3).GetComponent<BoxCollider>().size = new Vector3(1, 1, 4 * range);
      bomb.transform.GetChild(3).GetComponent<BombExplosion>().lifetime = bombLifetime;
      bomb.GetComponent<Bomb>().is3D = false;
      bomb.GetComponent<Bomb>().range = range;
    }
    if (cooldown != 0)
      cooldown--;
    // ok. koniec animacji
    if (cooldown == 0)
    {
      // wylaczenie animacji stawiania bomby
      animator.SetBool("isPlanting", false);
    }
  }

  // Update is called once per frame
  void Update()
  {
    Move();
    Bomb();
  }
}