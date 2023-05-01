using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public string token;
    public User user;
}

[Serializable]
public class User
{
    public string _id;
    public string nombre;
    public string cedula;
    public string telefono;
    public string email;
    public int edad;
    public string sexo;
    public float peso;
    public float altura;
    public string direccion;
    public string ciudad;
    public string __v;
}

/*[Serializable]
public class Patient
{
    public string _id;
    public string nombre;
    public string cedula;
    public string telefono;
    public string email;
    public int edad;
    public string sexo;
    public float peso;
    public float altura;
    public string direccion;
    public string ciudad;
    public string __v;
}*/

[Serializable]
public class Exercise
{
    public string _id;
    public string id_user;
    public string nombre;
    public int duracion_total;
    public int frecuencia_dias;
    public int frecuencia_horas;
    public int repeticiones;
    public int series;
    public int periodos_descanso;
    public string fecha_inicio;
    public string fecha_fin;
    public int apnea;
    public int flujo;
    public int hora_inicio;
    public string __v;
}

[Serializable]
public class Exercises
{
    public List<Exercise> array;
}

[Serializable]
public class ExerciseData
{
    public List<float> tiempo;
    public List<float> flujo;
}

[Serializable]
public class MotivationSound
{
    public string text;
    public AudioClip clip;
}

[System.Serializable]
public class ButtonsItems
{
    public GameObject useButton;
    public GameObject buyButton;
}

[System.Serializable]
public class AllItems
{
    public int[] item = new int[0];
}

public class ObjectsManager : MonoBehaviour
{

}
