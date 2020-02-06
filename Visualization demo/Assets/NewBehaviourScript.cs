using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class NewBehaviourScript : MonoBehaviour
{
    private List<string> KB_list = new List<string>();

    //define the flower
    public GameObject lotus_OBJ_low;
    public Vector3 growupSize;
    public Vector3 defaultSize;
    public Vector3 currentSize;
    public Vector3 defaultPosition;
    public Vector3 currentPostion;
    //public static bool runInBackground;

    //runInBackground = true;

    //wind for audio
    public float windSize;


    //fence for text
    public GameObject Fence;
    public Vector3 fenceSize;


    //water for action
    public Vector3 waterSize;
    public GameObject Can;

    //other parameters
    public string latest_data;
    public int Feedback_data;
    public int RGB_data;
    public int Audio_data;
    public int Text_data;
    public int KB_level;
    public Text RGB_text;
    public Text Audio_text;
    public Text Textforum;
    public Text KB_text;


    // Start is called before the first frame update

    void Start()
    {
        currentSize = defaultSize;
        currentPostion = defaultPosition;

        windSize = 0.0f; //how fast it shake

        //fence size into 0
        fenceSize.x = 0.0f;
        fenceSize.y = 0.0f;
        fenceSize.z = 0.0f;

        Fence.transform.localScale = fenceSize;

        //fence size into 0
        waterSize.x = 0.0f;
        waterSize.y = 0.0f;
        waterSize.z = 0.0f;

        Can.transform.localScale = waterSize;

        //flag
        RGB_data = 0;
        Audio_data = 0;
        Text_data = 0;
        KB_level = 0;
        RGB_text.text = "RGB feedback"+ RGB_data;
        Audio_text.text = "Audio feedback:" + Audio_data;
        Textforum.text = "Text feedback:" + Text_data;
        KB_text.text = "KB level:" + KB_level;

        //define KB level
        KB_list.Add(", idea generation");
        KB_list.Add(", idea connection");
        KB_list.Add(", idea improvement");
        KB_list.Add(", rise above");
    }

    // Update is called once per frame

    void Update()
    {

        // read data as stream
        var path = @"/Users/chenhaoyu/Desktop/MMdata_stream.txt";
        StreamReader r = new StreamReader(path);
        while (r.EndOfStream == false)
        {
            latest_data = r.ReadLine();
        }

        //give data to the feed back
        try
        {
            Feedback_data = Int32.Parse(latest_data);
        }
        catch (Exception)
        {
            Console.WriteLine("Start data stream!");
        }
        r.Close();



        /*take the action according to the feedback data*/
        
        switch (Feedback_data)
        {
            // audio feedback enable
            case 5:
                {
                    windSize = 10.0f;
                    Audio_data = 1;
                }
                break;
            // audio feedback diable
            case 6:
                {
                    windSize = 0.0f;
                    Audio_data = 0;
                }
                break;

            // video feedback enable
            case 7:
                {
                    waterSize.x = 0.07865506f;
                    waterSize.y = 0.07865506f;
                    waterSize.z = 0.07865506f;
                    RGB_data = 1;
                }
                break;
            // video feedback diable
            case 8:
                {
                    waterSize.x = 0.0f;
                    waterSize.y = 0.0f;
                    waterSize.z = 0.0f;
                    RGB_data = 0;
                }
                break;

            // text feedback enable
            case 9:
                {
                    fenceSize.x = 1.0f;
                    fenceSize.y = 1.0f;
                    fenceSize.z = 1.0f;
                    Text_data = 1;
                }
                break;

            // text feedback disable
            case 10:
                {
                    fenceSize.x = 0.0f;
                    fenceSize.y = 0.0f;
                    fenceSize.z = 0.0f;
                    Text_data = 0;
                }
                break;

            //KB level
            default:
                KB_level = Feedback_data;
                break;
        }

        FlowerAnimation();
        FenceAnimation();
        WaterAnimation();
        WindAnimation();


        RGB_text.text = "RGB feedback" + RGB_data;
        Audio_text.text = "Audio feedback:" + Audio_data;
        Textforum.text = "Text feedback:" + Text_data;
        KB_text.text = "KB level:" + KB_level.ToString() + KB_list.ElementAt(KB_level);

    }

    //flower animation
    void FlowerAnimation()
    {

        if (currentSize.x < (KB_level / 4.0 * growupSize.x))
        {
            currentSize.x = currentSize.x * (1.002f);
            currentSize.y = currentSize.y * (1.002f);
            currentSize.z = currentSize.z * (1.002f);
        }
        lotus_OBJ_low.transform.localScale = currentSize;
    }

    //wind animation for audio
    void WindAnimation()
    {
        if (Audio_data == 1)
        {
            
            currentPostion.x = Mathf.Sin(Time.time * windSize) * (KB_level+1)/4 *2.0f;
        }
        else
        {
            currentPostion.x = defaultPosition.x;
        }
        lotus_OBJ_low.transform.position = currentPostion;
    }

    // Fence animation for TEXT
    void FenceAnimation()
    {
        Fence.transform.localScale = fenceSize;
    }

    //Water animation for video
    void WaterAnimation()
    {
        Can.transform.localScale = waterSize;
    }

    void FlowerDeath()
    {

        if (currentSize.x > defaultSize.x)
        {
            currentSize.x = currentSize.x * (0.98f);
            currentSize.y = currentSize.y * (0.98f);
            currentSize.z = currentSize.z * (0.98f);
        }

            lotus_OBJ_low.transform.localScale = currentSize;
    }

}
