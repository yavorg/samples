package com.example.AndroidStarter;

import android.app.AlertDialog;
import android.app.Dialog;
import android.app.DialogFragment;
import android.content.DialogInterface;
import android.os.Bundle;

import com.google.common.util.concurrent.ListenableFuture;
import com.google.common.util.concurrent.SettableFuture;

import java.util.concurrent.ExecutionException;
import java.util.concurrent.Executor;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

public class OfflineConflictDialogFragment extends DialogFragment
        implements ListenableFuture<Integer>{
    private SettableFuture<Integer> settableFuture;

    public OfflineConflictDialogFragment() {
        settableFuture = SettableFuture.create();
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        final AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setMessage(R.string.offline_conflict)
                .setPositiveButton(R.string.offline_local, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        set(R.string.offline_local);
                        dismiss();

                    }
                })
                .setNegativeButton(R.string.offline_server, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        set(R.string.offline_server);
                        dismiss();
                    }
                });
        return builder.create();
    }

    @Override
    public void addListener(Runnable listener, Executor executor) {
        settableFuture.addListener(listener, executor);
    }

    @Override
    public boolean cancel(boolean b) {
        return settableFuture.cancel(b);
    }

    @Override
    public boolean isCancelled() {
        return settableFuture.isCancelled();
    }

    @Override
    public boolean isDone() {
        return settableFuture.isDone();
    }

    @Override
    public Integer get() throws InterruptedException, ExecutionException {
        return settableFuture.get();
    }

    @Override
    public Integer get(long timeout, TimeUnit unit) throws InterruptedException, ExecutionException, TimeoutException {
        return settableFuture.get(timeout, unit);
    }

    public void set(Integer value) {
        settableFuture.set(value);
    }

    @Override
    public void onDismiss(DialogInterface dialog) {
        cancel(true);
        super.onDismiss(dialog);
    }
}
