﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {
    static SerialPort _serialPort;

    SerialPort stream = new SerialPort("COM5", 115200);
    //set port and speed for seial communication


    public float swipeOrTap;

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.1f;
    float moveSpeed = 6;
    float gravity;

    Vector2 swipeTap = new Vector2(0, 0);
    
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;


    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        swipeTap.x = 1;//player is always moving

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); //setting up jump characteristics to be based on gravity, but controlled through jumpheight and time till apex
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);

        stream.Open();//open serial stream
    }

    void Update()
    {
       // SwipeTapControl();

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        //serial reading:
        string value = stream.ReadLine();
         swipeOrTap = float.Parse(value);

        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        

        if (swipeOrTap == 345)
        {
            swipeTap.x = -swipeTap.x;
        }

        if (swipeOrTap == 678  && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = swipeTap.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne); //smooth movements instead of directly input.x
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown("space") )
        {
            stream.Write("12");
            return;
        }
    }

    /// if 345 --> swiped
    /// if 678 --> tapped
    /// if 123 --> nothing


    //public static int SwipeTapControl()
    //{
    //    _serialPort = new SerialPort();
    //    _serialPort.PortName = "COM5";//Set your board COM
    //    _serialPort.BaudRate = 115200;
    //    _serialPort.Open();
    //    while (true)
    //    {
    //        string a = _serialPort.ReadExisting();
    //        Console.WriteLine(a);
    //        Thread.Sleep(200);
    //    }
    //}
}
