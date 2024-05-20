using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Newtonsoft.Json;
using System.Globalization;

[TestFixture]
public class Test_LocalLogin
{
    [Test]
    public void Test_LocalLogin_EmptyField()
    {        
        Login LoginObject = Setup_Login.LoginObject;
        NotificationsManager NotificationsManagerObject = Setup_Login.NotificationsManagerObject;

        // Establecer datos de prueba
        LoginObject.notInternet.isOn = true;
        LoginObject.userInputField.text = "";
        
        // Ejecutar la función
        LoginObject.ConexionMode();
        LoginObject.loginButton.onClick.Invoke();

        // Verificar
        Assert.IsTrue(NotificationsManagerObject.notificationsMenu.activeSelf);
        Assert.AreEqual("Ingresa los datos", NotificationsManagerObject.notificationsText.text);
        Assert.IsFalse(NotificationsManagerObject.notificationsYesButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNoButton.gameObject.activeSelf);
        Assert.IsTrue(NotificationsManagerObject.notificationsCloseButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNextButton.gameObject.activeSelf);        
    }

    [Test]
    public void Test_LocalLogin_NoExistsExerciseFile()
    {
        Login LoginObject = Setup_Login.LoginObject;
        GameData GameDataObject = Setup_Login.GameDataObject;
        NotificationsManager NotificationsManagerObject = Setup_Login.NotificationsManagerObject;

        // Establecer datos de prueba
        LoginObject.notInternet.isOn = true;
        LoginObject.userInputField.text = "1144207813";
        
        // Ejecutar la función
        File.Delete(GameDataObject.rutaArchivoFisioterapia);
        File.Delete(GameDataObject.rutaArchivoPredeterminado);
        File.Delete(GameDataObject.rutaArchivoPaciente);
        LoginObject.ConexionMode();
        LoginObject.loginButton.onClick.Invoke();        

        // Verificar
        Assert.IsTrue(NotificationsManagerObject.notificationsMenu.activeSelf);
        Assert.AreEqual("¡No tienes terapias! Por favor inicia sesión con usuario y contraseña para descargar la terapia", NotificationsManagerObject.notificationsText.text);
        Assert.IsFalse(NotificationsManagerObject.notificationsYesButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNoButton.gameObject.activeSelf);
        Assert.IsTrue(NotificationsManagerObject.notificationsCloseButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNextButton.gameObject.activeSelf);
    }

    [Test]
    public void Test_LocalLogin_NoPatientFile()
    {       
        Login LoginObject = Setup_Login.LoginObject;
        GameData GameDataObject = Setup_Login.GameDataObject;
        NotificationsManager NotificationsManagerObject = Setup_Login.NotificationsManagerObject;

        // Establecer datos de prueba
        LoginObject.notInternet.isOn = true;
        LoginObject.userInputField.text = "0";
        string today = DateTime.Today.ToString("dd/MM/yyyy");

        string exercise = $@"{{
            ""_id"":""6634fbc13d4204902b344a1e"",
            ""nombre"":""Inspiración profunda"",
            ""duracion_total"":3,
            ""frecuencia_dias"":1,
            ""frecuencia_horas"":1,
            ""repeticiones"":3,
            ""series"":2,""periodos_descanso"":3,
            ""fecha_inicio"":""{today}"",
            ""fecha_fin"":""{today}"",
            ""apnea"":3,
            ""flujo"":600,
            ""hora_inicio"":10,
            ""id_patient"":""66305f9d962af8471a72541c""
        }}";

        string defaultExercise = @"{
            ""_id"":""663062d5962af8471a72542f"",
            ""nombre"":""Predeterminado"",
            ""duracion_total"":1,
            ""frecuencia_dias"":3,
            ""frecuencia_horas"":3,
            ""repeticiones"":1,
            ""series"":1,
            ""periodos_descanso"":1,
            ""fecha_inicio"":null,
            ""fecha_fin"":null,
            ""apnea"":1,
            ""flujo"":600,
            ""hora_inicio"":6,
            ""id_patient"":""66305f9d962af8471a72541c""
        }";

        string patient = @"{
            ""token"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjZWR1bGEiOiIxMTQ0MjA3ODEzIiwiaWF0IjoxNzE0NzUyODY1LCJleHAiOjE3MTQ3NjM2NjV9.UmrZPmhNoQf5bdDWe_NrY32QE-zcDGNe_1iT8ptnT5o"",
            ""user"":{""_id"":""66305f9d962af8471a72541c"",
                ""nombre"":""Elizabeth fake"",
                ""cedula"":""1144207813"",
                ""telefono"":""3117019765"",
                ""email"":""biflora1@hotmail.com"",
                ""edad"":25,
                ""sexo"":""F"",
                ""peso"":56.0,
                ""altura"":156.0,
                ""direccion"":""trans 2 a 1 c 140"",
                ""ciudad"":""Santander, Piedecuesta"",
                ""id_user"":""66305f0bbbc6d046d1f4b107""
            }
        }";

        
        // Ejecutar la función
        File.WriteAllText(GameDataObject.rutaArchivoFisioterapia, exercise);
        File.WriteAllText(GameDataObject.rutaArchivoPredeterminado, defaultExercise);
        File.WriteAllText(GameDataObject.rutaArchivoPaciente, patient);
        LoginObject.ConexionMode();
        LoginObject.loginButton.onClick.Invoke();  

        // Verificar
        Assert.IsTrue(NotificationsManagerObject.notificationsMenu.activeSelf);
        Assert.AreEqual("El usuario no coincide con la fisioterapia asignada", NotificationsManagerObject.notificationsText.text);
        Assert.IsFalse(NotificationsManagerObject.notificationsYesButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNoButton.gameObject.activeSelf);
        Assert.IsTrue(NotificationsManagerObject.notificationsCloseButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNextButton.gameObject.activeSelf);
    }

    [Test]
    public void Test_LocalLogin_NoActiveExercise()
    {        
        Login LoginObject = Setup_Login.LoginObject;
        GameData GameDataObject = Setup_Login.GameDataObject;
        NotificationsManager NotificationsManagerObject = Setup_Login.NotificationsManagerObject;

        // Establecer datos de prueba
        LoginObject.notInternet.isOn = true;
        LoginObject.userInputField.text = "1144207813";
        string yesterday = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");

        string exercise = $@"{{
            ""_id"":"""",
            ""nombre"":""Predeterminado"",
            ""duracion_total"":1,
            ""frecuencia_dias"":3,
            ""frecuencia_horas"":3,
            ""repeticiones"":1,
            ""series"":1,
            ""periodos_descanso"":1,
            ""fecha_inicio"":""{yesterday}"",
            ""fecha_fin"":""{yesterday}"",
            ""apnea"":1,
            ""flujo"":600,
            ""hora_inicio"":6,
            ""id_patient"":""66305f9d962af8471a72541c""
        }}";

        string defaultExercise = @"{
            ""_id"":""663062d5962af8471a72542f"",
            ""nombre"":""Predeterminado"",
            ""duracion_total"":1,
            ""frecuencia_dias"":3,
            ""frecuencia_horas"":3,
            ""repeticiones"":1,
            ""series"":1,
            ""periodos_descanso"":1,
            ""fecha_inicio"":null,
            ""fecha_fin"":null,
            ""apnea"":1,
            ""flujo"":600,
            ""hora_inicio"":6,
            ""id_patient"":""66305f9d962af8471a72541c""
        }";

        string patient = @"{
            ""token"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjZWR1bGEiOiIxMTQ0MjA3ODEzIiwiaWF0IjoxNzE0NzUyODY1LCJleHAiOjE3MTQ3NjM2NjV9.UmrZPmhNoQf5bdDWe_NrY32QE-zcDGNe_1iT8ptnT5o"",
            ""user"":{""_id"":""66305f9d962af8471a72541c"",
                ""nombre"":""Elizabeth fake"",
                ""cedula"":""1144207813"",
                ""telefono"":""3117019765"",
                ""email"":""biflora1@hotmail.com"",
                ""edad"":25,
                ""sexo"":""F"",
                ""peso"":56.0,
                ""altura"":156.0,
                ""direccion"":""trans 2 a 1 c 140"",
                ""ciudad"":""Santander, Piedecuesta"",
                ""id_user"":""66305f0bbbc6d046d1f4b107""
            }
        }";
        
        // Ejecutar la función
        File.WriteAllText(GameDataObject.rutaArchivoFisioterapia, exercise);
        File.WriteAllText(GameDataObject.rutaArchivoPredeterminado, defaultExercise);
        File.WriteAllText(GameDataObject.rutaArchivoPaciente, patient);
        LoginObject.ConexionMode();
        LoginObject.loginButton.onClick.Invoke();

        // Verificar
        Assert.IsTrue(NotificationsManagerObject.notificationsMenu.activeSelf);
        Assert.AreEqual("¡No tienes terapias! Por favor inicia sesión con usuario y contraseña para descargar la terapia", NotificationsManagerObject.notificationsText.text);
        Assert.IsFalse(NotificationsManagerObject.notificationsYesButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNoButton.gameObject.activeSelf);
        Assert.IsTrue(NotificationsManagerObject.notificationsCloseButton.gameObject.activeSelf);
        Assert.IsFalse(NotificationsManagerObject.notificationsNextButton.gameObject.activeSelf);
    }

    [Test]
    public void Test_LocalLogin_InactiveExercise()
    {
        Login LoginObject = Setup_Login.LoginObject;
        GameData GameDataObject = Setup_Login.GameDataObject;
        ExercisesManager ExerciseManagerObject = Setup_Login.ExerciseManagerObject;
        RewardsManager RewardsManagerObject = Setup_Login.RewardsManagerObject;
        CustomizationManager CustomizationManagerObject = Setup_Login.CustomizationManagerObject;
        
        // Establecer datos de prueba
        LoginObject.notInternet.isOn = true;
        LoginObject.userInputField.text = "1144207813";
        string yesterday = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");

        string exercise = $@"{{
            ""_id"":""6634fbc13d4204902b344a1e"",
            ""nombre"":""Inspiración profunda"",
            ""duracion_total"":3,
            ""frecuencia_dias"":1,
            ""frecuencia_horas"":1,
            ""repeticiones"":3,
            ""series"":2,""periodos_descanso"":3,
            ""fecha_inicio"":""{yesterday}"",
            ""fecha_fin"":""{yesterday}"",
            ""apnea"":3,
            ""flujo"":600,
            ""hora_inicio"":10,
            ""id_patient"":""66305f9d962af8471a72541c""
        }}";

        string defaultExercise = @"{
            ""_id"":""663062d5962af8471a72542f"",
            ""nombre"":""Predeterminado"",
            ""duracion_total"":1,
            ""frecuencia_dias"":3,
            ""frecuencia_horas"":3,
            ""repeticiones"":1,
            ""series"":1,
            ""periodos_descanso"":1,
            ""fecha_inicio"":null,
            ""fecha_fin"":null,
            ""apnea"":1,
            ""flujo"":600,
            ""hora_inicio"":6,
            ""id_patient"":""66305f9d962af8471a72541c""
        }";

        string patient = @"{
            ""token"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjZWR1bGEiOiIxMTQ0MjA3ODEzIiwiaWF0IjoxNzE0NzUyODY1LCJleHAiOjE3MTQ3NjM2NjV9.UmrZPmhNoQf5bdDWe_NrY32QE-zcDGNe_1iT8ptnT5o"",
            ""user"":{""_id"":""66305f9d962af8471a72541c"",
                ""nombre"":""Elizabeth fake"",
                ""cedula"":""1144207813"",
                ""telefono"":""3117019765"",
                ""email"":""biflora1@hotmail.com"",
                ""edad"":25,
                ""sexo"":""F"",
                ""peso"":56.0,
                ""altura"":156.0,
                ""direccion"":""trans 2 a 1 c 140"",
                ""ciudad"":""Santander, Piedecuesta"",
                ""id_user"":""66305f0bbbc6d046d1f4b107""
            }
        }";

        string custom = @"{
            ""_id"":""66305fa0962af8471a72541f"",
            ""id_customization"":0,
            ""id_item_fondos_array"":""0,-1,-1,-1,-1"",
            ""id_item_figuras_array"":""0,-1,-1,-1,-1"",
            ""all_fondos_items_array"":""1,1,1;0,0,0;0,0,0;0,0,0;0,0,0;"",
            ""all_figuras_items_array"":""1,1,1;0,0,0;0,0,0;0,0,0;0,0,0;"",
            ""id_patient"":""66305f9d962af8471a72541c""
        }";

        string reward = @"{
            ""_id"":""66305fa0962af8471a725421"",
            ""all_badges_array"":""1,0,0,0,0,0,0;1,1,0,0,0,0,0;1,0,0,0,0,0,0;0,0,0,0,0,0,0;"",
            ""session_reward"":500,
            ""day_reward"":300,
            ""total_reward"":20425,
            ""total_series"":17,
            ""total_sessions"":15,
            ""total_days"":2,
            ""total_weeks"":0,
            ""id_patient"":""66305f9d962af8471a72541c""
        }";
                
        // Ejecutar la función
        File.WriteAllText(GameDataObject.rutaArchivoFisioterapia, exercise);
        File.WriteAllText(GameDataObject.rutaArchivoPredeterminado, defaultExercise);
        File.WriteAllText(GameDataObject.rutaArchivoPaciente, patient);
        File.WriteAllText(GameDataObject.rutaArchivoRecompensa, reward);
        File.WriteAllText(GameDataObject.rutaArchivoPersonalizacion, custom);

        Exercise e = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameDataObject.rutaArchivoFisioterapia));
        GameDataObject.jsonObjectExerciseDefault = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameDataObject.rutaArchivoPredeterminado));

        bool active = (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(e.fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(e.fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture)) ? true : false;

        if(!active && GameDataObject.jsonObjectExerciseDefault.nombre != e.nombre)
        {
            e = JsonUtility.FromJson<Exercise>(defaultExercise);
            e._id = "";
            e.fecha_inicio = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            e.fecha_fin = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(e.frecuencia_dias - 1).ToString("dd/MM/yyyy");

            ExercisesManager.Instance.UpdateLocalExercise(GameData.Instance.rutaArchivoFisioterapia, JsonConvert.SerializeObject(e));
            GameData.Instance.jsonObjectExercises = JsonConvert.DeserializeObject<List<Exercise>>("["+File.ReadAllText(GameData.Instance.rutaArchivoFisioterapia)+"]");
            GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(File.ReadAllText(GameData.Instance.rutaArchivoRecompensa));
            GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(File.ReadAllText(GameData.Instance.rutaArchivoPersonalizacion));
        }        

        // Verificar
        Assert.AreEqual(JsonConvert.SerializeObject(GameData.Instance.jsonObjectExercises), JsonConvert.SerializeObject(GameDataObject.jsonObjectExercises));
        Assert.AreEqual(JsonConvert.SerializeObject(GameData.Instance.jsonObjectRewards),JsonConvert.SerializeObject(GameDataObject.jsonObjectRewards));
        Assert.AreEqual(JsonConvert.SerializeObject(GameData.Instance.jsonObjectCustomizations), JsonConvert.SerializeObject(GameDataObject.jsonObjectCustomizations));
        Assert.AreEqual(UI_System.Instance.CurrentScreen, ExerciseManagerObject.sessionMenu);
    }

    [Test]
    public void Test_LocalLogin_ActiveExercise()
    {
        Login LoginObject = Setup_Login.LoginObject;
        GameData GameDataObject = Setup_Login.GameDataObject;
        ExercisesManager ExerciseManagerObject = Setup_Login.ExerciseManagerObject;
        RewardsManager RewardsManagerObject = Setup_Login.RewardsManagerObject;
        CustomizationManager CustomizationManagerObject = Setup_Login.CustomizationManagerObject;
        
        // Establecer datos de prueba
        LoginObject.notInternet.isOn = true;
        LoginObject.userInputField.text = "1144207813";
        string today = DateTime.Today.ToString("dd/MM/yyyy");

        string exercise = $@"{{
            ""_id"":""6634fbc13d4204902b344a1e"",
            ""nombre"":""Inspiración profunda"",
            ""duracion_total"":3,
            ""frecuencia_dias"":1,
            ""frecuencia_horas"":1,
            ""repeticiones"":3,
            ""series"":2,""periodos_descanso"":3,
            ""fecha_inicio"":""{today}"",
            ""fecha_fin"":""{today}"",
            ""apnea"":3,
            ""flujo"":600,
            ""hora_inicio"":10,
            ""id_patient"":""66305f9d962af8471a72541c""
        }}";

        string defaultExercise = @"{
            ""_id"":""663062d5962af8471a72542f"",
            ""nombre"":""Predeterminado"",
            ""duracion_total"":1,
            ""frecuencia_dias"":3,
            ""frecuencia_horas"":3,
            ""repeticiones"":1,
            ""series"":1,
            ""periodos_descanso"":1,
            ""fecha_inicio"":null,
            ""fecha_fin"":null,
            ""apnea"":1,
            ""flujo"":600,
            ""hora_inicio"":6,
            ""id_patient"":""66305f9d962af8471a72541c""
        }";

        string patient = @"{
            ""token"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjZWR1bGEiOiIxMTQ0MjA3ODEzIiwiaWF0IjoxNzE0NzUyODY1LCJleHAiOjE3MTQ3NjM2NjV9.UmrZPmhNoQf5bdDWe_NrY32QE-zcDGNe_1iT8ptnT5o"",
            ""user"":{""_id"":""66305f9d962af8471a72541c"",
                ""nombre"":""Elizabeth fake"",
                ""cedula"":""1144207813"",
                ""telefono"":""3117019765"",
                ""email"":""biflora1@hotmail.com"",
                ""edad"":25,
                ""sexo"":""F"",
                ""peso"":56.0,
                ""altura"":156.0,
                ""direccion"":""trans 2 a 1 c 140"",
                ""ciudad"":""Santander, Piedecuesta"",
                ""id_user"":""66305f0bbbc6d046d1f4b107""
            }
        }";

        string custom = @"{
            ""_id"":""66305fa0962af8471a72541f"",
            ""id_customization"":0,
            ""id_item_fondos_array"":""0,-1,-1,-1,-1"",
            ""id_item_figuras_array"":""0,-1,-1,-1,-1"",
            ""all_fondos_items_array"":""1,1,1;0,0,0;0,0,0;0,0,0;0,0,0;"",
            ""all_figuras_items_array"":""1,1,1;0,0,0;0,0,0;0,0,0;0,0,0;"",
            ""id_patient"":""66305f9d962af8471a72541c""
        }";

        string reward = @"{
            ""_id"":""66305fa0962af8471a725421"",
            ""all_badges_array"":""1,0,0,0,0,0,0;1,1,0,0,0,0,0;1,0,0,0,0,0,0;0,0,0,0,0,0,0;"",
            ""session_reward"":500,
            ""day_reward"":300,
            ""total_reward"":20425,
            ""total_series"":17,
            ""total_sessions"":15,
            ""total_days"":2,
            ""total_weeks"":0,
            ""id_patient"":""66305f9d962af8471a72541c""
        }";
                
        // Ejecutar la función
        File.WriteAllText(GameDataObject.rutaArchivoFisioterapia, exercise);
        File.WriteAllText(GameDataObject.rutaArchivoPredeterminado, defaultExercise);
        File.WriteAllText(GameDataObject.rutaArchivoPaciente, patient);
        File.WriteAllText(GameDataObject.rutaArchivoRecompensa, reward);
        File.WriteAllText(GameDataObject.rutaArchivoPersonalizacion, custom);

        Exercise e = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameDataObject.rutaArchivoFisioterapia));
        GameDataObject.jsonObjectExerciseDefault = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameDataObject.rutaArchivoPredeterminado));

        bool active = (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(e.fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(e.fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture)) ? true : false;

        if(active)
        {
            ExercisesManager.Instance.UpdateLocalExercise(GameData.Instance.rutaArchivoFisioterapia, JsonConvert.SerializeObject(e));
            GameData.Instance.jsonObjectExercises = JsonConvert.DeserializeObject<List<Exercise>>("["+File.ReadAllText(GameData.Instance.rutaArchivoFisioterapia)+"]");
            GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(File.ReadAllText(GameData.Instance.rutaArchivoRecompensa));
            GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(File.ReadAllText(GameData.Instance.rutaArchivoPersonalizacion));
        }        

        // Verificar
        Assert.AreEqual(JsonConvert.SerializeObject(GameData.Instance.jsonObjectExercises), JsonConvert.SerializeObject(GameDataObject.jsonObjectExercises));
        Assert.AreEqual(JsonConvert.SerializeObject(GameData.Instance.jsonObjectRewards),JsonConvert.SerializeObject(GameDataObject.jsonObjectRewards));
        Assert.AreEqual(JsonConvert.SerializeObject(GameData.Instance.jsonObjectCustomizations), JsonConvert.SerializeObject(GameDataObject.jsonObjectCustomizations));
        Assert.AreEqual(UI_System.Instance.CurrentScreen, ExerciseManagerObject.sessionMenu);
    }
}