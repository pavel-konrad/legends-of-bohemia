using UnityEngine;
using System.Collections.Generic;

public class SpellQueuePresenter : MonoBehaviour
{
    [SerializeField] private SpellQueueView _view;
    
    private PlayerController _player;

    public void SetPlayer(PlayerController player)
    {
        _player = player;
        _player.OnQueueChanged += UpdateView;
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.OnQueueChanged -= UpdateView;
    }



    private void UpdateView(Queue<ISpell> queue)
    {
        _view.UpdateQueue(queue);
    }
}