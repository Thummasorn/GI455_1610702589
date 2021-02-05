using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindWord : MonoBehaviour
{
    public string Animals;
    public string[] Animal = new string[5];
    public Text result;
    public InputField search;

  
    public void OtherAnimal()
    {
        Animals = search.text;
        
        if (Animals == Animal[0])
        {
            result.text = " [ " + $"<color=green>{Animals}</color>" + " ] " + " Is found ";

        }
        else if (Animals == Animal[1])
        {
            result.text = " [ " + $"<color=green>{Animals}</color>" + " ] " + " Is found ";
        }
        else if (Animals == Animal[2])
        {
            result.text = " [ " + $"<color=green>{Animals}</color>" + " ] " + " Is found ";
        }
        else if (Animals == Animal[3])
        {
            result.text = " [ " + $"<color=green>{Animals}</color>" + " ] " + " Is found ";
        }
        else if (Animals == Animal[4])
        {
            result.text = " [ " + $"<color=green>{Animals}</color>" + " ] " + " Is found ";
        }
        else
        {
            result.text = " [ " + $"<color=red>{Animals}</color>" + " ] " + " Is Not found ";
        }
    }
    

    



}
