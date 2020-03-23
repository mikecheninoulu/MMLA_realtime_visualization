using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MMLAscript : MonoBehaviour
{

    //canvas
    //public CanvasGroup canvasGroup;

    // define the plants list by using the concept of object pool
    private List<GameObject> plantsObjects;      //plant list
    private int currentIndex; //index of the pool
    private int plantsnum = 26;

    //define the plant color level list
    List<int> plantcolor_list = new List<int>();

    //define the plant default color material index list
    List<int> defaultcolor  = new List<int>() { 0, 2, 3, 7, 5, 8};

    //define the plants
    public GameObject pineTree;
    public GameObject pineTree02;
    public GameObject pineTree03;
    public GameObject pineTree04;
    public GameObject pineTree05;
    public GameObject pineTree06;
    public GameObject pineTree07;
    public GameObject pineTree08;
    public GameObject pineTree09;
    public GameObject pineTree10;
    public GameObject pineTree11;
    public GameObject pineTree12;
    public GameObject pineTree13;
    public GameObject pineTree14;
    public GameObject pineTree15;
    public GameObject pineTree16;
    public GameObject pineTree17;
    public GameObject pineTree18;
    public GameObject pineTree19;
    public GameObject pineTree20;
    public GameObject pineTree21;
    public GameObject pineTree22;
    public GameObject pineTree23;
    public GameObject pineTree24;
    public GameObject pineTree25;
    public GameObject pineTree26;

    //size of the default tree
    //define the temp size of the plants
    public List<Vector3> defaultSize_list = new List<Vector3>();

    public Vector3 defaultSize;

    //define the temp size of the plants
    public List<Vector3> currentSize_list = new List<Vector3>();
    public Vector3 currentSize;

    // initialize the moutain
    public GameObject mountain;
    public GameObject mountain2;
    public GameObject mountain3;

    // initialize the plane
    public GameObject plane;

    // initialize the scene canvas
    public Text Video_text;
    public Text Audio_text;
    public Text Context_text;
    public Text CL_text;
    public Text CL_text1;

    // other parameters
    //stream data
    public string latest_data;
    public string Video_feedback;
    public string Audio_feedback;
    public string Context_feedback;

    // feedback array, m*n n = person, m = modality
    public int stream_data;
    public int video_flag1;
    public int video_flag2;
    public int audio_flag;
    public int audio_over_flag;
    public int context_flag;
    public int context_level;
    public int CL_flag;
    public int showup_index;

    //public int[] Video_feedback_array = {0};
    //public int[] Audio_feedback_array = {0};
    //public int[] Context_feedback_array = {0};

    //collaborative learning level
    public int CL_level;
    //the list of the level
    private List<string> CL_list = new List<string>();

    //define colors
    public UnityEngine.Object[] materials;
    public UnityEngine.Object[] materialsky;

    //define renderer
    Renderer mountainrend;
    Renderer mountain2rend;
    Renderer mountain3rend;
    Renderer planerend;
    Renderer plantrend;

    System.Random rnd = new System.Random();

    // Start is called before the first frame update
    void Start()
    {

        //canvasGroup.alpha = 0f;
        //load all the materials
        materials = Resources.LoadAll("Materials", typeof(Material));
        materialsky = Resources.LoadAll("Materialsky", typeof(Material));
        //print(materials.Length);

        // create a plant pool
        plantsObjects = new List<GameObject>();
        //load plants into plant list
        plantsObjects.Add(pineTree);
        plantsObjects.Add(pineTree02);
        plantsObjects.Add(pineTree03);
        plantsObjects.Add(pineTree04);
        plantsObjects.Add(pineTree05);
        plantsObjects.Add(pineTree06);
        plantsObjects.Add(pineTree07);
        plantsObjects.Add(pineTree08);
        plantsObjects.Add(pineTree09);
        plantsObjects.Add(pineTree10);
        plantsObjects.Add(pineTree11);
        plantsObjects.Add(pineTree12);
        plantsObjects.Add(pineTree13);
        plantsObjects.Add(pineTree14);
        plantsObjects.Add(pineTree15);
        plantsObjects.Add(pineTree16);
        plantsObjects.Add(pineTree17);
        plantsObjects.Add(pineTree18);
        plantsObjects.Add(pineTree19);
        plantsObjects.Add(pineTree20);
        plantsObjects.Add(pineTree21);
        plantsObjects.Add(pineTree22);
        plantsObjects.Add(pineTree23);
        plantsObjects.Add(pineTree24);
        plantsObjects.Add(pineTree25);
        plantsObjects.Add(pineTree26);

        // load defaultsizes into size pool
        defaultSize_list = new List<Vector3>();

        // load currentsizes into size pool
        currentSize_list = new List<Vector3>();

        //initial the sizes of the plants
        for (int i = 0; i < plantsnum; ++i)
        {
            //store the default size
            defaultSize = plantsObjects[i].transform.localScale;
            defaultSize_list.Add(defaultSize);

            //initial the sizes of the plants
            currentSize.x = defaultSize.x * (0.5f);
            currentSize.y = defaultSize.y * (0.5f);
            currentSize.z = defaultSize.z * (0.5f);
            currentSize_list.Add(currentSize);
            //initial the sizes of the plants to 0.1

            //render each plant
            plantrend = plantsObjects[i].GetComponent<Renderer>();
            plantrend.sharedMaterial = (Material)materials[0];
            plantcolor_list.Add(0);

            plantsObjects[i].SetActive(false);    //make all the plant deactivate first
            
        }
        

        //initialize the data from the stream
        Video_feedback = "No movement!";
        Audio_feedback = "No conversation!";
        Context_feedback = "No discourse!";
        CL_level = 1;

        //flag initialize
        video_flag1 = 0;
        video_flag2 = 0;
        audio_flag = 0;
        audio_over_flag = 0;
        context_flag = 0;
        context_level = 0;
        CL_flag = 0;
        showup_index = 0;

        //set the text in the canvas
        //Video_text.text = "Video: "+ Video_feedback;
        //Audio_text.text = "Audio: " + Audio_feedback;
        //Context_text.text = "Lexical Audio: " + Context_feedback;
        CL_text.text = "Collaborative phase: 1";
        CL_text1.text = "Idea Generation.";

        //define KB level
        CL_list.Add("Idea Generation.");
        CL_list.Add("Idea Connection.");
        CL_list.Add("Idea Improvement.");
        CL_list.Add("Rise Above.");


        //render the scene
        //load colors
        mountainrend = mountain.GetComponent<Renderer>();
        mountainrend.sharedMaterial = (Material)materials[14];
        //mountainrend.sharedMaterials[1] = (Material)materials[12];

        mountain2rend = mountain2.GetComponent<Renderer>();
        mountain2rend.sharedMaterial = (Material)materials[14];

        mountain3rend = mountain3.GetComponent<Renderer>();
        mountain3rend.sharedMaterial = (Material)materials[14];

        //render the plane
        planerend = plane.GetComponent<Renderer>();
        planerend.material = (Material)materials[8];

        //render the sky
        RenderSettings.skybox = (Material)materialsky[6];

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
            //stream formatting
            //person 1      person 2       person3
            //action audio action audio action audio
            //3       0      6      0     0     0

            String[] data_list = latest_data.Split(' ');
            // feedback array, m*n n = person, m = modality

            //temp feedback 
            stream_data = int.Parse(latest_data);
            //print(stream_data);
            //Context_feedback_array[0] = int.Parse(data_list[2]);

        }
        catch (Exception)
        {
            Console.WriteLine("Start data stream wrong!");
        }
        r.Close();

        if (stream_data == 1)
        {

        }

        //Inference
        //Inference video
        //no video feedback
        //int row1Sum = feedback_array[,].Cast<int>().Sum();

        switch (stream_data)
        {
            case 0:
                //Video_text.text = "Video: " + "No movement!";
                //Audio_text.text = "Audio: " + "No conversation!";
                //Context_text.text = "Lexical Audio: " + "No discourse!";
                video_flag1 = 0;
                video_flag2 = 0;
                audio_over_flag = 0;
                audio_flag = 0;
                break;

            case 1:
                //Video_text.text = "Video: " + "Significant movement!";
                //more seed animation
                video_flag1 += 1;
                if (video_flag1 > 90)
                {
                    PlantAddseedAnimation();

                    video_flag1 = 0;
                }
                break;

            case 2:
                
                //more color animation
                audio_flag += 1;
                if (audio_flag > 120)
                {
                    audio_flag = 0;
                    audio_over_flag += 1;
                    if (audio_over_flag > 10)
                    {
                        PlantOverColorAnimation();
                        //Audio_text.text = "Audio: " + "Overdomained talking!";
                    }
                    else
                    {
                        PlantColorAnimation();
                        //Audio_text.text = "Audio: " + "Normal conversation!";
                    }
                }
                break;

            case 3:
                //more seed animation
                video_flag2 += 1;
                if (video_flag2 > 20)
                {
                    video_flag2 = 0;
                    //growth animation
                    PlantGrowthAnimation();
                }
                //Context_text.text = "Lexical Audio: " + "Meaningful discourse!";

                break;

            case 10:
                //Video_text.text = "Video: " + "No movement!";
                video_flag1 = 0;
                video_flag2 = 0;
                //stop growth animation
                break;

            case 20:
                //Audio_text.text = "Audio: " + "No conversation!";
                audio_over_flag = 0;
                audio_flag = 0;
                //stop growth animation
                PlantRecoverColorAnimation();

                break;

            case 9:
                CL_flag += 1;
                if (CL_flag > 200)
                {
                    if(CL_level<4)
                    {
                        CL_level += 1;
                        MoutainAnimation();
                        CL_flag = 0;
                    }

                }

                CL_text.text = "Collaborative phase: " + CL_level.ToString();
                CL_text1.text = CL_list.ElementAt(CL_level-1);
                //stop growth animation
                break;

            case 100:
                Start();
                break;

            default:
                print("wrong input!");
                break;
        }

    }

    //sub functions
     
    //plant seed adding animation
    void PlantAddseedAnimation()
    {
        plantsObjects[showup_index].SetActive(true);
        plantsObjects[showup_index].transform.localScale = currentSize_list[showup_index];
        //print(currentSize_list[showup_index].x);
        //print(currentSize_list[showup_index].y);
        //print(currentSize_list[showup_index].z);

        showup_index += 1;
    }

    //plant growth animation
    void PlantGrowthAnimation()
    {
        //initial the sizes of the plants
        for (int i = 0; i < showup_index; ++i)
        {
            if (currentSize_list[i].x < defaultSize_list[i].x)
            {
                currentSize.x = currentSize_list[i].x * (1.02f);
                currentSize.y = currentSize_list[i].y * (1.02f);
                currentSize.z = currentSize_list[i].z * (1.02f);
            }
            else
            {
                currentSize = defaultSize_list[i];
            }
            //print(i);
            //print(defaultSize);

            plantsObjects[i].transform.localScale = currentSize;

            currentSize_list[i] = currentSize;
        }
    }

    //plant growth animation
    void PlantColorAnimation()
    {
        //initial the sizes of the plants
        for (int i = 0; i < showup_index; ++i)
        {
            int current_colorindex = plantcolor_list[i];

            current_colorindex += 1;

            if (current_colorindex < 4)
            {
                plantcolor_list[i] = current_colorindex;
                plantrend = plantsObjects[i].GetComponent<Renderer>();
                plantrend.sharedMaterial = (Material)materials[defaultcolor[current_colorindex]];
            }
        }
    }


    //plant growth animation
    void PlantRecoverColorAnimation()
    {
        //initial the sizes of the plants
        for (int i = 0; i < showup_index; ++i)
        {
            int current_colorindex = plantcolor_list[i];
            
            if (current_colorindex > 3)
            {
                current_colorindex -= 1;
                plantcolor_list[i] = current_colorindex;
                plantrend = plantsObjects[i].GetComponent<Renderer>();
                plantrend.sharedMaterial = (Material)materials[defaultcolor[current_colorindex]];
            }
        }
    }

    //plant growth animation
    void PlantOverColorAnimation()
    {
        //initial the sizes of the plants
        for (int i = 0; i < showup_index; ++i)
        {
            int current_colorindex = plantcolor_list[i];
            current_colorindex += 1;
            if (current_colorindex < 6)
            {
                plantcolor_list[i] = current_colorindex;
                plantrend = plantsObjects[i].GetComponent<Renderer>();
                plantrend.sharedMaterial = (Material)materials[defaultcolor[current_colorindex]];
            }
        }
    }

    //plant seed adding animation
    void MoutainAnimation()
    {
        switch (CL_level)
        {
            case 2:
                //render the moutain
                mountainrend = mountain.GetComponent<Renderer>();
                mountainrend.sharedMaterial = (Material)materials[10];

                mountain2rend = mountain2.GetComponent<Renderer>();
                mountain2rend.sharedMaterial = (Material)materials[10];

                mountain3rend = mountain3.GetComponent<Renderer>();
                mountain3rend.sharedMaterial = (Material)materials[10];

                planerend = plane.GetComponent<Renderer>();
                planerend.material = (Material)materials[5];
                break;

            case 3:
                //render the moutain
                mountainrend = mountain.GetComponent<Renderer>();
                mountainrend.sharedMaterial = (Material)materials[3];

                mountain2rend = mountain2.GetComponent<Renderer>();
                mountain2rend.sharedMaterial = (Material)materials[2];

                mountain3rend = mountain3.GetComponent<Renderer>();
                mountain3rend.sharedMaterial = (Material)materials[10];

                planerend = plane.GetComponent<Renderer>();
                planerend.material = (Material)materials[7];

                //render the sky
                RenderSettings.skybox = (Material)materialsky[1];

                break;

            case 4:
                //render the moutain
                mountainrend = mountain.GetComponent<Renderer>();
                mountainrend.sharedMaterial = (Material)materials[2];

                mountain2rend = mountain2.GetComponent<Renderer>();
                mountain2rend.sharedMaterial = (Material)materials[3];

                mountain3rend = mountain3.GetComponent<Renderer>();
                mountain3rend.sharedMaterial = (Material)materials[3];

                planerend = plane.GetComponent<Renderer>();
                planerend.material = (Material)materials[7];

                RenderSettings.skybox = (Material)materialsky[8];
                break;

            default:
                print("context level problem!");
                break;
        }

    }


}
